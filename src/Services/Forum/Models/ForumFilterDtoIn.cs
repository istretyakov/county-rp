﻿namespace CountyRP.Services.Forum.Models
{
    public class ForumFilterDtoIn : PagedFilter
    {
        public int ParentId { get; }

        public ForumFilterDtoIn(
            int count, 
            int page,
            int parentId
        ) 
            : base(count, page)
        {
            ParentId = parentId;
        }
    }
}
