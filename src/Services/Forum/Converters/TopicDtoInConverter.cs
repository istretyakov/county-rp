﻿using CountyRP.Services.Forum.Entities;
using CountyRP.Services.Forum.Models;

namespace CountyRP.Services.Forum.Converters
{
    public class TopicDtoInConverter
    {
        public static TopicDao ToDb(
            TopicDtoIn source
        )
        {
            return new TopicDao(
                id: 0,
                caption: source.Caption,
                forumId: source.ForumId
            );
        }

        public static TopicDtoOut ToDtoOut(
            TopicDtoIn source,
            int id
        )
        {
            return new TopicDtoOut(
                id: id,
                caption: source.Caption,
                forumId: source.ForumId
            );
        }
    }
}
