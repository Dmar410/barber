using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AppointmentApp.Controllers
{
    [Route("appointments")]
    public class AppointmentsController : Controller
    {
        private static List<Appointment> appointments = new List<Appointment>();

        [HttpGet("get-started")]
        public IActionResult GetStarted()
        {
            return View();
        }

        [HttpGet("make-appointment")]
        public IActionResult MakeAppointment()
        {
            return View();
        }

        [HttpPost("add")]
        public IActionResult AddAppointment(Appointment newAppointment)
        {
            newAppointment.Id = appointments.Count + 1;
            appointments.Add(newAppointment);
            return RedirectToAction("GetAppointments");
        }

        [HttpGet("appointments-list")]
        public IActionResult GetAppointments()
        {
            return View(appointments);
        }
    }

    public class Appointment
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Date { get; set; }
    public required string Time { get; set; }
    public required string Email { get; set; }
}
}