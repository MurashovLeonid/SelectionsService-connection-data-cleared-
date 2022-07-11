using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Superbrands.Libs.DDD.EfCore.Abstractions;
using Superbrands.Selection.Domain.Enums;
using Superbrands.Selection.Infrastructure.DAL;
using Superbrands.Selection.Infrastructure.RepositoryResponses;

namespace Superbrands.Selection.Infrastructure.Abstractions
{
    public interface IProcurementRepository : IRepository<ProcurementDalDto>
    {
        void Update(ProcurementDalDto procurementDalDto);
        Task<IEnumerable<ColorModelMetaDalDto>> GetProductsByGroupingParameters(GroupKeyType groupKeyType, int groupKeyId,
            CancellationToken cancellationToken);

        Task<List<ColorModelMetaDalDto>> GetProductsByProcurementSalePointIds(long procurementId, long salePointId,
            CancellationToken cancellationToken);

        Task<IEnumerable<ColorModelMetaDalDto>> GetProductsByGroupingParameters(int procurementId, GroupKeyType groupKeyType,
            int groupKeyId, CancellationToken cancellationToken);

        IEnumerable<SelectionDalDto> GetFilteredSelections(long procurementId, BTSelectionsGroupResponse responseFromBt,
            CancellationToken cancellationToken);

        IEnumerable<ProcurementDalDto> GetFilteredProcurements(BTSelectionsGroupResponse responseFromBt,
            CancellationToken cancellationToken);

        Task<NomenclatureTemplateDalDto> CreateNomenclatureTemplate(NomenclatureTemplateDalDto nomenclatureTemplate,
            CancellationToken cancellationToken);

        Task<List<NomenclatureTemplateDalDto>> GetAllNomenclatureTemplates(CancellationToken cancellationToken);
        Task<NomenclatureTemplateDalDto> GetNomenclatureTemplateById(int templateId, CancellationToken cancellationToken);
        Task<List<ProcurementDalDto>> GetProcurementsByIds(IEnumerable<long> ids, CancellationToken cancellationToken);
        Task<List<ProcurementDalDto>> GetByIds(List<long> ids, CancellationToken cancellationToken);
        void EditRange(List<ProcurementDalDto> procurements);
        Task<List<ProcurementDalDto>> GetAll(CancellationToken cancellationToken);
        Task<List<ColorModelMetaDalDto>> GetProductStatisticQuery(long procurementsId, List<string> modelVendoreCodes, CancellationToken cancellationToken);
        Task<List<ProcurementDalDto>> GetBySalePointIds(List<long> ids, CancellationToken cancellationToken);
        Task<List<ProcurementDalDto>> GetProcurementsBySalePointAttributes(long businessSegmentId, List<long> seasonIds, long partnerId, CancellationToken cancellationToken);
        Task<List<ProcurementDalDto>> GetBySeasonCapsuleIds(List<long> seasonCapsuleIds,
            CancellationToken cancellationToken);
    }

}