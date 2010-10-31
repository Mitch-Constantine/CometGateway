using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspComet;
using System.Reflection;

namespace CometGateway.Server.Gateway
{
    public class MessageMap
    {
        public IEnumerable<KeyValuePair<string, Type>> MessageTypeMap { get; private set; }

        public MessageMap(IEnumerable<KeyValuePair<string, Type>> messageTypeMap)
        {
            this.MessageTypeMap = messageTypeMap;
        }

        public MessageMap(Assembly assembly, string namespaceName)
        {
            MessageTypeMap = assembly
                                .GetTypes()
                                .Where(type => type.Namespace == namespaceName)
                                .Select(MatchWithTypeCode)
                                .Where( pair => pair.Key != null )
                                .ToList();
        }

        private KeyValuePair<string, Type> MatchWithTypeCode(Type type)
        {
            object[] typeCodeAttribute = type.GetCustomAttributes(typeof(MessageTypeAttribute), false);
            if (typeCodeAttribute.Length == 0)
                return new KeyValuePair<string, Type>(null, type);
            MessageTypeAttribute typeCode = typeCodeAttribute.Single() as MessageTypeAttribute;
            return new KeyValuePair<string,Type>(typeCode.TypeCode, type);
        }

        public Message Encode(object toEncode)
        {
            string messageType = FindMessageType(toEncode.GetType());

            Message message = new Message();
            message.SetData("type", messageType);
            CopyMembersToMessage(message, toEncode);
            return message;
        }

        public object Decode(Message message)
        {
            string typeCode = message.GetData<string>("type");

            Type objectType = FindObjectType(typeCode);
            var toDecode = Activator.CreateInstance(objectType);
            CopyMessageToMembers(toDecode, message);
            return toDecode;
        }

        private string FindMessageType(Type type)
        {
            return MessageTypeMap
                        .Single(pair => pair.Value.Equals(type))
                        .Key;
        }

        private Type FindObjectType(string typeCode)
        {
            return MessageTypeMap
                        .Single(pair => pair.Key == typeCode)
                        .Value;
        }

        private void CopyMembersToMessage(Message message, object toEncode)
        {
            foreach (string fieldNameCrt in GetFieldsOf(toEncode))
            {
                message.SetData(fieldNameCrt, GetFieldValue(toEncode, fieldNameCrt));
            }

            foreach (string propertyNameCrt in GetPropertiesOf(toEncode))
            {
                message.SetData(propertyNameCrt, GetPropertyValue(toEncode, propertyNameCrt));
            }
        }

        private void CopyMessageToMembers(object toDecode, Message message)
        {
            var data = message.data as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> propertyValueCrt in data)
            {
                SetProperty(toDecode, propertyValueCrt.Key, propertyValueCrt.Value);
            }
        }

        private void SetProperty(object toDecode, string propertyName, object propertyValue)
        {
            Type type = toDecode.GetType();
            var field = type.GetField(propertyName);
            if (field != null)
            {
                var castedValue = Convert.ChangeType(propertyValue, field.FieldType);
                field.SetValue(toDecode, castedValue);
                return;
            }
            var property = type.GetProperty(propertyName);
            if (property != null)
            {
                var castedValue = Convert.ChangeType(propertyValue, property.PropertyType);
                property.SetValue(toDecode, propertyValue, null);
            }
        }

        private IEnumerable<string> GetFieldsOf(object toEncode)
        {
            return toEncode.GetType()
                           .GetFields()
                           .Select(field => field.Name);
        }

        private object GetFieldValue(object toEncode, string fieldName)
        {
            return toEncode.GetType()
                           .GetField(fieldName)
                           .GetValue(toEncode);
        }

        private IEnumerable<string> GetPropertiesOf(object toEncode)
        {
            return toEncode.GetType()
                           .GetProperties()
                           .Select(property => property.Name);
        }

        private object GetPropertyValue(object toEncode, string propertyName)
        {
            return toEncode.GetType()
                           .GetProperty(propertyName)
                           .GetValue(toEncode, null);
        }
    }
}
