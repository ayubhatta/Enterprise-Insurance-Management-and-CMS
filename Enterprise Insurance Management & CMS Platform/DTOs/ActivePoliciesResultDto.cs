namespace Enterprise_Insurance_Management___CMS_Platform.DTOs
{
    public class ActivePoliciesResultDto
    {
        public int TotalActivePolicies { get; set; }
        public IEnumerable<PolicyResponseDto>? Policies { get; set; }
    }

}
