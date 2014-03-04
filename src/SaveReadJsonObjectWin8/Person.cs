using System;

namespace SaveReadJsonObjectWin8
{
    public class Person : Identifiers
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    
    public abstract class Identifiers
    {
        public Guid Id { get; set; }

        internal Identifiers()
        {
            if (Id == new Guid())
                Id = Guid.NewGuid();
        }
    }
}
