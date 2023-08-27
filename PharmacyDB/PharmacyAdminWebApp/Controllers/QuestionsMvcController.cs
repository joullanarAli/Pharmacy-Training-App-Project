using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmacyDB.Interfaces;
using PharmacyDB.Models;
using PharmacyInfrastructure.Requests;
using System.Text;
using System.Text.Json;

namespace PharmacyAdminWebApp.Controllers
{
    public class QuestionsMvcController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public QuestionsMvcController(HttpClient httpClient, IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment) : base(unitOfWork)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5191/Questions");
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> IndexAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Questions");
          //  var httpClient = _httpClientFactory.CreateClient("Questions");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Question>>(responseStream, options);
                return View(data);
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> GetQuestions()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Questions");
            var httpClient = _httpClientFactory.CreateClient("Questions");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Question>>(responseStream, options);
                // return View(data);
                return Json(new
                {
                    success = true,
                    message = "All Data is back",
                    data = data
                });
            }
            else
            {
                // Handle error cases
                return View("Error");
            }
        }
        public async Task<IActionResult> SearchQuestion(string value)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/Questions/Search?value=" + value);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Question>>(responseStream, options);
                return Json(new
                {
                    success = true,
                    message = "All Data is back",
                    data = data
                });
            }
            else
            {
                return View("Error");
            }
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Question _question)
        {
            _question.CourseId = 1;
             _question.ExamQuestionList=new List<ExamQuestion>();
            
            
            Question question = new Question()
            {
                QuestionText = _question.QuestionText,
                WrongAnswerMark = _question.WrongAnswerMark,
                CorrectAnswerMark = _question.CorrectAnswerMark,
                NoAnswerMark = _question.NoAnswerMark,
                CourseId = 1,
            };

            var content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/Questions", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Question>>(responseStream, options);
                    TempData["Message"] = "Data Created Successfully";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["Error"] = "An error occurred while creating the exam.";
                    return View();

                }
            
        }
        public async Task<IActionResult> GetModal(int id)
        {
            Question question = await _unitOfWork._questionRepository.GetById(id);
            return PartialView("Modal", question);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Question _question)
        {

            Question question = new Question
            {
                Id = _question.Id,
                QuestionText = _question.QuestionText,
                WrongAnswerMark = _question.WrongAnswerMark,
                CorrectAnswerMark = _question.CorrectAnswerMark,
                NoAnswerMark = _question.NoAnswerMark,
                CourseId = _question.CourseId,
            };
            var content = new StringContent(JsonConvert.SerializeObject(question), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync("/Questions", content);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Question>>(responseStream, options);
                TempData["Message"] = "Data Updated Successfully";

                return RedirectToAction("Index");

            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            return PartialView("Delete", await _unitOfWork._questionRepository.GetById(id));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteQuestionMvc(int id)
        {
            Question question = (await _unitOfWork._questionRepository.GetById(id));
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/Questions?questionId={id}");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Question>>(responseStream, options);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> GetDetails(int id)
        {
            return PartialView("Details", await _unitOfWork._questionRepository.GetById(id));
        }
        public async Task<IActionResult> Choices(int id)
        {
            var choices = (await _unitOfWork._choiceRepository.GetAll()).Where(element => element.QuestionId == id).ToList();
            return View("Questionchoices",choices);
        }
        public async Task<IActionResult> GetQuestionChoices(int questionId)
        {
            var choices = (await _unitOfWork._choiceRepository.GetAll()).Where(element => element.QuestionId == questionId);
            return Json(new
            {
                success = true,
                message = "All Data is back",
                data = choices
            });
        }
        public async Task<IActionResult> AddChoice(int questionId)
        {
            var choices=(await _unitOfWork._choiceRepository.GetAll()).Where(e => e.QuestionId==questionId).ToList();
            return View("AddChoice");
        }
        public async Task<IActionResult> AddChoiceToQuestion(PharmacyDB.Models.Choice _choice)
        {
            Choice choice = new Choice()
            {
                ChoiceText = _choice.ChoiceText,
                QuestionId = _choice.QuestionId,
            };

            var content = new StringContent(JsonConvert.SerializeObject(choice), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/Choices", content);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Choice>>(responseStream, options);
                TempData["Message"] = "Data Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {

                TempData["Error"] = "An error occurred while creating the exam.";
                return View();

            }
        }
        public async Task<IActionResult> GetChoiceModal(int id)
        {
            PharmacyDB.Models.Choice choice = await _unitOfWork._choiceRepository.GetById(id);
            return PartialView("ChoiceModal", choice);
        }
        [HttpPost]
        public async Task<IActionResult> EditChoice(PharmacyDB.Models.Choice _choice)
        {

            PharmacyDB.Models.Choice choice = new PharmacyDB.Models.Choice
            {
                Id = _choice.Id,
                ChoiceText = _choice.ChoiceText,
                QuestionId = _choice.QuestionId,
                Score = _choice.Score,
            };
            var content = new StringContent(JsonConvert.SerializeObject(choice), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync("/Choices", content);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<PharmacyDB.Models.Choice>>(responseStream, options);
                TempData["Message"] = "Data Updated Successfully";

                return RedirectToAction("Index");

            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> DeleteChoiceModal(int id)
        {
            return PartialView("DeleteChoice", await _unitOfWork._choiceRepository.GetById(id));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteChoiceMvc(int id)
        {
            PharmacyDB.Models.Choice choice = (await _unitOfWork._choiceRepository.GetById(id));
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/Choices?choiceId={id}");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<PharmacyDB.Models.Choice>>(responseStream, options);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }
    }
}
