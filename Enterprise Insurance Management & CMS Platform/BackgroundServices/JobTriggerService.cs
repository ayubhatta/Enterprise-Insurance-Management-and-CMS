using Hangfire;
using Enterprise_Insurance_Management___CMS_Platform.BackgroundServices;

namespace Enterprise_Insurance_Management___CMS_Platform.Services
{
    public class JobTriggerService(JobService _jobService)
    {
        // New User Registered
        public void TriggerNewUserRegisteredJob(string userId, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.NewUserRegisteredJob(userId), delay);
        }

        // Customer Verified
        public void TriggerCustomerVerifiedJob(string userId, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.CustomerVerifiedJob(userId), delay);
        }

        // Policy Created
        public void TriggerPolicyCreatedJob(Guid policyId, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.PolicyCreatedJob(policyId), delay);
        }

        // Policy Purchased
        public void TriggerPolicyPurchasedJob(Guid customerPolicyId, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.PolicyPurchasedJob(customerPolicyId), delay);
        }

        // Policy Payment Confirmed
        public void TriggerPolicyPaymentConfirmedJob(Guid customerPolicyId, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.PolicyPaymentConfirmedJob(customerPolicyId), delay);
        }

        // Claim Submitted
        public void TriggerClaimSubmittedJob(Guid claimId, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.ClaimSubmittedJob(claimId), delay);
        }

        // Claim Status Updated
        public void TriggerClaimStatusUpdatedJob(Guid claimId, string newStatus, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => _jobService.ClaimStatusUpdatedJob(claimId, newStatus), delay);
        }
    }
}
