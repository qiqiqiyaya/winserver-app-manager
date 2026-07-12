using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace AppManager.IisSites;

public interface IIisInstanceAppService : IApplicationService
{
    Task<PagedResultDto<IisInstanceDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<IisInstanceDto> GetAsync(Guid id);
    Task<IisInstanceDto> CreateAsync(CreateIisInstanceDto input);
    Task<IisInstanceDto> UpdateAsync(Guid id, UpdateIisInstanceDto input);
    Task DeleteAsync(Guid id);
}
