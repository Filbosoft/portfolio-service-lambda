using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Business.Commands.Common
{
    public class UpdateCommand : BusinessRequest
    {

        public object GetUpdatedValues()
        {
            dynamic updatedProperties = new ExpandoObject();
            
            foreach (var property in this.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(this);

                if (propertyValue == null) continue;
                
                ((IDictionary<string, object>)updatedProperties).Add(property.Name, propertyValue);
            }

            return updatedProperties;
        }

        public T TransferUpdatedValues<T>(T entity)
        {
            foreach (var property in this.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(this);

                if (propertyValue == null) continue;
                
                var entityProperty = entity.GetType().GetProperty(property.Name);
                if (entityProperty == null || !entityProperty.CanWrite) continue;

                entityProperty.SetValue(entity, propertyValue, null);
            }

            return entity;
        }
    }
}