namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class InActivePoliciesResultDto
    {
        public int TotalInActivePolicies { get; set; }
        public IEnumerable<PolicyResponseDto>? Policies { get; set; }
    }

}
