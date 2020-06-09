using System;


namespace TwitchBot.Common.Models.Entity
{
    /// <summary>
    /// A base class for entities, providing a typical set of common fields.    
    /// Database-specific configuration (such as string length, isRequired, etc) is defined using fluent-API
    /// Please do not add database specific annotations to these models!
    /// </summary>
    public class EntityBase : IEntityBase
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
