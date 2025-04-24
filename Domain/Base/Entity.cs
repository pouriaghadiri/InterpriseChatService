using Domain.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base
{
    public class Entity : IEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; private set; }
        public int CreatedBy { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int ModifiedBy { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime DeletedAt { get; private set; }
        public bool IsArchived { get; private set; }

        public Entity()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public void Update()
        {
            UpdatedAt = DateTime.Now;
        }
        public void Delete()
        {
            var now = DateTime.Now;
            IsActive = false;
            IsDeleted = true;
            DeletedAt = now;
            UpdatedAt = now;
        }
        
        public void DeActive() 
        {
            var now = DateTime.Now;
            IsActive = false; 
            UpdatedAt = now;
        }
        public void Activate() 
        {
            var now = DateTime.Now;
            IsActive = true; 
            UpdatedAt = now;
        }

    }
}
