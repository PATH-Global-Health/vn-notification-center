using System;
using System.Collections.Generic;
using System.Text;

namespace Data.MongoCollections
{
    public class Notification : BaseMongoCollection
    {
        public bool Seen { get; set; }
        public string Action { get; set; }

        public Guid UserId { get; set; }
    }
}
