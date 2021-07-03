﻿using CountyRP.Services.Site.Models;
using System.Threading.Tasks;

namespace CountyRP.Services.Site.Repositories
{
    public partial interface ISiteRepository
    {
        Task<SupportRequestTopicDtoOut> AddSupportRequestTopicAsync(SupportRequestTopicDtoIn supportRequestTopicDtoIn);

        Task<SupportRequestTopicDtoOut> GetSupportRequestTopicAsync(int id);

        Task<PagedFilterResult<SupportRequestTopicDtoOut>> GetSupportRequestTopicsByFilterAsync(SupportRequestTopicFilterDtoIn filter);

        Task<PagedFilterResult<SupportRequestTopicWithFirstAndLastMessagesDtoOut>> GetSupportRequestTopicsByFilterByLastMessagesAsync(SupportRequestTopicFilterDtoIn filter);

        Task<SupportRequestTopicDtoOut> UpdateSupportRequestTopicAsync(SupportRequestTopicDtoOut supportRequestTopicDtoOut);

        Task DeleteSupportRequestTopicAsync(int id);
    }
}
