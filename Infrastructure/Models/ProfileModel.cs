using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public class ProfileModel
    {
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }
        public IList<HttpModel> Endpoints { get; set; }
    }
}
