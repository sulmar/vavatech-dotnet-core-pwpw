using System;
using System.Collections.Generic;
using System.Text;

namespace Vavatech.DotnetCore.Models.SearchCriterias
{

    public abstract class SearchCriteria
    {

    }
    public class CustomerSearchCriteria : SearchCriteria
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
