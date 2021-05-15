﻿namespace CountyRP.Services.Forum.Models
{
    public class TopicFilterDtoIn : PagedFilter
    {
        public int ForumId { get; }

        public TopicFilterDtoIn(
            int count, 
            int page,
            int forumId
        ) 
            : base(count, page)
        {
            ForumId = forumId;
        }
    }
}
