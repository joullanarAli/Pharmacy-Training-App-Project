using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmacyDB.Interfaces;
using PharmacyDB.Models;
using PharmacyInfrastructure.Shared;
using System.Text;
using System.Text.Json;

namespace PharmacyAdminWebApp.Controllers
{
    public class ActiveIngredientsMvcController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ActiveIngredientsMvcController(HttpClient httpClient, IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment) :base(unitOfWork)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5191/ActiveIngredients");
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> IndexAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/ActiveIngredients");
            var httpClient = _httpClientFactory.CreateClient("ActiveIngredients");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ActiveIngredient>>(responseStream, options);
                return View(data);
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> GetActiveIngredients()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/ActiveIngredients");
            var httpClient = _httpClientFactory.CreateClient("ActiveIngredients");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ActiveIngredient>>(responseStream, options);
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
        public async Task<IActionResult> SearchActiveIngredient(string value)
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/ActiveIngredients/Search?value=" + value);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ActiveIngredient>>(responseStream, options);
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
        public async Task<IActionResult> Create(ActiveIngredient _activeIngredient)
        {
            if (ModelState.IsValid)
            {
                ActiveIngredient activeIngredient = new ActiveIngredient()
                {
                    Name = _activeIngredient.Name,
                };
                /*ActiveIngredientRequest requestData = new ActiveIngredientRequest
                {
                    Id = activeIngredient.Id,
                    Name = activeIngredient.Name
                };*/
                var content = new StringContent(JsonConvert.SerializeObject(activeIngredient), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/ActiveIngredients", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ActiveIngredient>>(responseStream, options);
                    /* _unitOfWork.SaveChanges();
                     var brands = (await _unitOfWork._brandRepository.GetAll()).Reverse().ToList();
                     await _unitOfWork._brandRepository.Add(activeIngredient);
                     _unitOfWork.SaveChanges();
                     brands = (await _unitOfWork._brandRepository.GetAll()).Reverse().ToList();*/
                    TempData["Message"] = "Data Created Successfully";
                    return RedirectToAction("Index");
                }
                else
                {

                    TempData["Error"] = "An error occurred while creating the brand.";
                    return View();

                }
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> GetModal(int id)
        {
            ActiveIngredient activeIngredient = await _unitOfWork._activeIngredientRepository.GetById(id);
            return PartialView("Modal", activeIngredient);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ActiveIngredient _activeIngredient)
        {

            ActiveIngredient activeIngredient = new ActiveIngredient
            {
                Id = _activeIngredient.Id,
                Name = _activeIngredient.Name,
            };
            /*ActiveIngredientRequest requestData = new ActiveIngredientRequest
            {
                Id = activeIngredient.Id,
                Name = activeIngredient.Name
            };*/
            var content = new StringContent(JsonConvert.SerializeObject(activeIngredient), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync("/ActiveIngredients", content);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ActiveIngredient>>(responseStream, options);
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
            return PartialView("Delete", await _unitOfWork._activeIngredientRepository.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteActiveIngredientMvc(int id)
        {
            ActiveIngredient activeIngredient = (await _unitOfWork._activeIngredientRepository.GetById(id));
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/ActiveIngredients?activeIngredientId={id}");
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ActiveIngredient>>(responseStream, options);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> Drugs(int id)
        {
            ActiveIngredient activeIngredient = await _unitOfWork._activeIngredientRepository.GetById(id);
            return View(activeIngredient);
        }
        public async Task<IActionResult> GetActiveIngredientDrugs(int activeIngredientId)
        {
            var drugsActiveIngredients = (await _unitOfWork._drugActiveIngredientRepository.GetAll()).Where(e => e.ActiveIngredientId== activeIngredientId).ToList();
            List<Drug> drugs=new List<Drug>();
            foreach(var drugActiveIngredient in drugsActiveIngredients)
            {
                Drug drug = await _unitOfWork._drugRepository.GetById(drugActiveIngredient.DrugId);
                drugs.Add(drug);
            }
            // var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<DrugForm>>(responseStream, options);
            return Json(new
            {
                success = true,
                message = "All Data is back",
                data = drugs
            });

        }

    }
}
