using GINS_Task.DAL;
using GINS_Task.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace GINS_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingDetailsController : ControllerBase
    {
        private MeetingDetailsDAL _meetingDetailsDAL;
        public MeetingDetailsController()
        {
            _meetingDetailsDAL = new MeetingDetailsDAL();
        }

        [HttpGet("customerName")]
        public IActionResult GetCustomerName(string Type)
        {
            List<Customer> customers = (List<Customer>)_meetingDetailsDAL.GetCustomerName(Type);

            return Ok(customers);
        }

        [HttpGet("productService")]
        public IActionResult GetProductService()
        {
            List<ProductService> products = (List<ProductService>)_meetingDetailsDAL.GetProductService();

            return Ok(products);
        }

        [HttpPost("saveMeetingMinutes")]
        public IActionResult SaveMeetingMinutes([FromBody] MeetingMinutesViewModel vModel)
        {
            try
            {
                int meetingMinutesID = _meetingDetailsDAL.SaveMeetingMinutes(vModel);

                _meetingDetailsDAL.InsertMeetingMinutesProducts(meetingMinutesID, vModel.MeetingMunitesProducts);

                return Ok("Meeting minutes saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving meeting minutes: {ex.Message}");
            }
           
        }
    }
}
