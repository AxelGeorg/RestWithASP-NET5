using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestWithASPNETUdemy.Business;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Model;

namespace RestWithASPNETUdemy.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]/v{version:apiVersion}")]
    public class AuthController : ControllerBase
    {
        private ILoginBusiness _loginBusiness;

        public AuthController(ILoginBusiness loginBusiness)
        {
            _loginBusiness = loginBusiness;
        }

        [HttpPost]
        [Route("signin")]
        public IActionResult Signin([FromBody] UserVO user)
        {
            if (user == null)
                return BadRequest("Invalid client request");

            TokenVO token = _loginBusiness.ValidateCredentials(user);
            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] TokenVO tokenVO)
        {
            if (tokenVO == null)
                return BadRequest("Invalid client request");

            TokenVO token = _loginBusiness.ValidateCredentials(tokenVO);
            if (token == null)
                return BadRequest("Invalid client request");

            return Ok(token);
        }

        [HttpGet]
        [Route("revoke")]
        [Authorize("Bearer")]
        public IActionResult Revoke()
        {
            string userName = User.Identity.Name;
            
            bool result = _loginBusiness.RevokeToken(userName);
            if (!result)
                return BadRequest("Invalid client request");

            return NoContent();
        }
    }
}
