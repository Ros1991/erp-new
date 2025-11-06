using System;

namespace CAU.CrossCutting.Exceptions
{
    /// <summary>
    /// Base exception for all domain-specific exceptions
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception for when a requested entity is not found
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public EntityNotFoundException(string entityName, object entityId)
            : base($"{entityName} with ID '{entityId}' was not found.")
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public EntityNotFoundException(string entityName, string fieldName, object fieldValue)
            : base($"{entityName} with {fieldName} '{fieldValue}' was not found.")
        {
            EntityName = entityName;
            EntityId = fieldValue;
        }
    }

    /// <summary>
    /// Exception for business rule violations
    /// </summary>
    public class BusinessRuleException : DomainException
    {
        public string RuleName { get; }
        
        public BusinessRuleException(string ruleName, string message) : base(message)
        {
            RuleName = ruleName;
        }
    }

    /// <summary>
    /// Exception for validation errors
    /// </summary>
    public class ValidationException : DomainException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors) 
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }

        public ValidationException(string field, string error) 
            : base($"Validation error in field '{field}': {error}")
        {
            Errors = new Dictionary<string, string[]>
            {
                { field, new[] { error } }
            };
        }
    }

    /// <summary>
    /// Exception for duplicate entity conflicts
    /// </summary>
    public class DuplicateEntityException : DomainException
    {
        public string EntityName { get; }
        public string FieldName { get; }
        public object FieldValue { get; }

        public DuplicateEntityException(string entityName, string fieldName, object fieldValue)
            : base($"{entityName} with {fieldName} '{fieldValue}' already exists.")
        {
            EntityName = entityName;
            FieldName = fieldName;
            FieldValue = fieldValue;
        }
    }

    /// <summary>
    /// Exception for unauthorized access
    /// </summary>
    public class UnauthorizedDomainException : DomainException
    {
        public UnauthorizedDomainException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception for forbidden operations
    /// </summary>
    public class ForbiddenOperationException : DomainException
    {
        public ForbiddenOperationException(string message) : base(message) { }
    }
}
