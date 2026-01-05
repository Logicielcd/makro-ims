using Makro.IMS.Infra.Data.Auth;
using Makro.IMS.Infra.Data.Context;
using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Infra.Data.UnitOfWorks;
using Makro.IMS.Services.Api.Dto;
using Makro.IMS.Services.Api.Sieve;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sieve.Models;
using Sieve.Services;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace Makro.IMS.Services.Api.Services
{
    public class SmsService
    {
        private UnitOfWork unitOfWork;
        private readonly IMSContext imsContext;
        private readonly ISieveProcessor _sieveProcessor;

        public SmsService()
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);            
        }

        public SmsService(
            ISieveProcessor sieveProcessor
            )
        {
            imsContext = imsContext ?? new IMSContext();
            unitOfWork = new UnitOfWork(imsContext);
            _sieveProcessor = sieveProcessor;
        }
  
        public async Task<string> SendSms(SmsDto sms)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var smsSetting = config.GetSection("SmsSetting");
            string endpoint = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "Endpoint").Value;
            string serviceId = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "ServiceId").Value;
            string user = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "User").Value;
            string pass = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "Pass").Value;
            string senderName = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "SenderName").Value;
            string shortCode = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "ShortCode").Value;
            string billingNumber = smsSetting.GetChildren().FirstOrDefault(x => x.Key == "BillingNumber").Value;


            string SID = serviceId; // "2323921100"; //Service Id
            string User = user; // "2323921100"; //User
            string Pass = pass; // "NcM@B23wU4C!jvx"; //Password
            string SenderName = senderName; // "CPXBOOKING";
            string ShortCode = shortCode; // "40002359";   //ShortCode
            string BillingNumber = billingNumber; // "66942135633";   //Billing Number

            string DstMobileNo = sms.TelNo; //"66958039339";   //Mobile No.
            string SmsText = sms.SmsMessage; //"ทดสอบการส่งข้อมูล SMS from API";

            string EncodeBase64 = EncodeTo64(User + ":" + Pass);
            string apiResponse = "";

            using (var httpClient = new HttpClient())
            {
                XDocument xDocument = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?><message>"
                    + "<sms type =\"mt\"><service-id>" + SID + "</service-id>"
                    + "<destination><address><number type=\"international\">" + DstMobileNo + "</number></address></destination>"
                    + "<source><address><number type=\"abbreviated\">" + ShortCode + "</number>"
                    + "<originate type=\"international\">" + BillingNumber + "</originate>"
                    + "<sender>" + SenderName + "</sender></address></source>"
                    //+ "<ud type=\"text\" encoding=\"default\">" + EngText + "</ud>"
                    + "<ud type=\"text\" encoding=\"unicode\">" + ConvertToDecimalNCR(SmsText) + "</ud>"
                    + "<scts>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK") + "</scts>"
                    + "<dro>true</dro></sms></message>");

                StringContent content = new StringContent(xDocument.ToString(), Encoding.UTF8, "text/xml");

                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + EncodeBase64);
                httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");

                //url endpoint can be:
                //http://bulksms.truebiz.space:55000  for http on test environment
                //https://bulksms.truebiz.space  for https on test environment
                //http://119.46.177.99:55000  for http on production environment
                //https://bulk.truecorp.co.th  for https on production environment

                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    response = await httpClient.PostAsync(endpoint, content);

                    apiResponse = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    apiResponse = ex.Message;
                }
                finally
                {
                    response.Dispose();
                }
            }

            return apiResponse;
        }

        public string EncodeTo64(string toEncode)

        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        public string ConvertToDecimalNCR(string input)
        {
            StringBuilder ncrText = new StringBuilder();
            foreach (char c in input)
            {
                ncrText.AppendFormat("&#{0};", (int)c);
            }
            return ncrText.ToString();
        }
    }
}
