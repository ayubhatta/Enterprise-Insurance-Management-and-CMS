using Enterprise_Insurance_Management___CMS_Platform.Entities;

namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class ExpiredPoliciesResultDto
    {
        public int TotalExpiredPolicies { get; set; }
        public IEnumerable<PolicyResponseDto>? Policies { get; set; }
    }

}
