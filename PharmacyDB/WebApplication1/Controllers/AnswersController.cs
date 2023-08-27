/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyDB.Interfaces;
using PharmacyDB.Models;

namespace PharmacyWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AnswersController : BaseController
    {
        public AnswersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        [HttpGet(Name ="GetAllAnswers")]
        public async Task<IActionResult> GetAllAnswers()
        {
            try
            {
                var answers=(await _unitOfWork._answerRepository.GetAll()).Reverse().ToList();
                return Ok(answers);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost(Name ="CreateAnswer")]
        public async Task<IActionResult> CreateAnswer(Answer answer)
        {
            try
            {
                await _unitOfWork._answerRepository.Add(answer);
                _unitOfWork.SaveChanges();
                var answers = (await _unitOfWork._answerRepository.GetAll()).Reverse().ToList();
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut(Name ="UpdateAnswer")]
        public async Task<IActionResult> UpdateAnswer(int answerId,Answer answer)
        {
            try
            {
                Answer _answer=await _unitOfWork._answerRepository.GetById(answerId);
                _answer.AnswerText=answer.AnswerText;
                _answer.ChoiceId=answer.ChoiceId;
              //  _answer.UserQuestionsId=answer.UserQuestionsId;
                _unitOfWork.SaveChanges();
                var answers = (await _unitOfWork._answerRepository.GetAll()).Reverse().ToList();
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete(Name ="DeleteAnswer")]
        public async Task<IActionResult> DeleteAnswer(int answerId)
        {
            try
            {
                Answer answer = await _unitOfWork._answerRepository.GetById(answerId);
                _unitOfWork._answerRepository.Delete(answer);
                _unitOfWork.SaveChanges();
                var answers = (await _unitOfWork._answerRepository.GetAll()).Reverse().ToList();
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
*/