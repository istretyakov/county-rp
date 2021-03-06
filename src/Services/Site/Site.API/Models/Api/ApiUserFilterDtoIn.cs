﻿using System.Collections.Generic;

namespace CountyRP.Services.Site.API.Models.Api
{
    public class ApiUserFilterDtoIn : ApiPagedFilter
    {
        public string Login { get; set; }

        public IEnumerable<string> GroupIds { get; set; }
    }
}
