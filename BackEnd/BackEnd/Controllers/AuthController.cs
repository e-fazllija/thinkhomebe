using AutoMapper;
using BackEnd.Entities;
using BackEnd.Interfaces;
using BackEnd.Models.AuthModels;
using BackEnd.Models.MailModels;
using BackEnd.Models.ResponseModel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("/api/[controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMailService mailService, IConfiguration configuration, IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _mailService = mailService;
            _configuration = configuration;
            this._mapper = mapper;
        }

        [HttpPost]
        [Route(nameof(SendEmail))]
        public async Task<IActionResult> SendEmail()
        {
            MailRequest mailRequest = new MailRequest()
            {
                ToEmail = "elsad3631@gmail.com",
                Subject = "test",
                Body = "test"
            };
            await _mailService.SendEmailAsync(mailRequest);
            return Ok();
        }


        [HttpPost]
        [Route(nameof(Register))]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userEmailExists = await userManager.FindByEmailAsync(model.Email);
            var userNameExists = await userManager.FindByNameAsync(model.Username);
            if (userEmailExists != null || userNameExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "Utente già registrato!" });


            model.Username = model.Username.Replace(" ", "_");

            ApplicationUser user = new ApplicationUser()
            {
                Name = model.Name,
                LastName = model.Lastname,
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded && result.Errors.First().Code == "PasswordRequiresUpper")
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel { Status = "Error", Message = "La password deve contenere almeno una lettera maiuscola!" });
            else if (!result.Succeeded && result.Errors.First().Code == "PasswordRequiresNonAlphanumeric")
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel { Status = "Error", Message = "La password deve contenere almeno un carattere speciale!" });
            else if (!result.Succeeded && result.Errors.First().Code == "PasswordTooShort")
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel { Status = "Error", Message = "La password deve contenere almeno 8 caratteri!" });
            else if (!result.Succeeded && result.Errors.First().Code == "PasswordRequiresDigit")
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel { Status = "Error", Message = "La password deve contenere almeno un numero!" });

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel { Status = "Error", Message = "Si è verificato un errore! Controllare i dati inseriti e provare nuovamente" });

            var roleResult = await userManager.AddToRoleAsync(user, model.Role);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:5173/metronic8/vue/demo1/#/email-confirmation/{user.Email}/{token}";
                //Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);
            MailRequest mailRequest = new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Conferma la tua email",
                Body = $"Per attivare le tue credenziali <a href='{confirmationLink}'>clicca qui</a>"
            };
            await _mailService.SendEmailAsync(mailRequest);

            return Ok(new AuthResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route(nameof(RegisterAdmin))]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            model.Username = model.Username.Replace(" ", "_");

            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "User already exists!" });
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = "",
                LastName = ""
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));
            if (await roleManager.RoleExistsAsync("Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            return Ok(new AuthResponseModel { Status = "Success", Message = "User created successfully!" });
        }


        [HttpPost]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login(LoginModel model)
        {
            model.Email = model.Email.Replace(" ", "_");
            var user = await userManager.FindByEmailAsync(model.Email);
            var pass = await userManager.CheckPasswordAsync(user, model.Password);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
                var token = new JwtSecurityToken(
                        issuer: _configuration["Authentication:Issuer"],
                    audience: _configuration["Authentication:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                LoginResponse result = new LoginResponse()
                {
                    Name = user.Name,
                    Lastname = user.LastName,
                    Email = user.Email,
                    Password = "",
                    Role = userRoles.FirstOrDefault() ?? "",
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                };

                return Ok(result);
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "Utente non autorizzato" });
        }

        [HttpPost]
        [Route(nameof(VerifyToken))]
        public async Task<IActionResult> VerifyToken(TokenVerificationModel api_token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["Authentication:Issuer"],
                    ValidAudience = _configuration["Authentication:Audience"],

                    ClockSkew = TimeSpan.Zero // Imposta lo skew dell'orologio a zero per evitare eventuali problemi di sincronizzazione
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(api_token.api_token, validationParameters, out validatedToken);
                string? email;
                if (principal.Identity.IsAuthenticated)
                {
                    email = principal.Claims.First().Value;
                    var user = await userManager.FindByEmailAsync(email);
                    var userRoles = await userManager.GetRolesAsync(user);
                    LoginResponse result = new LoginResponse()
                    {
                        Name = user.Name,
                        Lastname = user.LastName,
                        Email = user.Email,
                        Password = "",
                        Role = userRoles.FirstOrDefault() ?? "",
                        Token = api_token.api_token
                    };

                    return Ok(result);
                }

                return Unauthorized();
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(nameof(ConfirmCredentials))]
        public async Task<IActionResult> ConfirmCredentials(CredentialConfirmationModel credentials)
        {
            var user = await userManager.FindByEmailAsync(credentials.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "User not exists!" });

            var result = await userManager.ConfirmEmailAsync(user, credentials.Token);

            if(result.Succeeded)
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "Si è verificato un errore" });
        }

        [HttpGet]
        [Route(nameof(KeyGen))]
        public async Task<IActionResult> KeyGen()
        {
            byte[] keyBytes = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }

            string secretKey = Convert.ToBase64String(keyBytes);
            return Ok(secretKey);
        }

        [HttpPost]
        [Route(nameof(CreateRole))]
        public async Task<IActionResult> CreateRole(string role)
        {
            try
            {
                IdentityRole identityRole = new IdentityRole()
                {
                    Name = role
                };
                var roleAdded = await roleManager.CreateAsync(identityRole);

                return Ok(roleAdded);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        //[HttpGet]
        //[Route(nameof(ForgotPassword))]
        //public async Task<IActionResult> ForgotPassword(string email)
        //{
        //    var user = await userManager.FindByEmailAsync(email);
        //    if (user == null) return NotFound("NOT_FOUND");

        //    var token = await userManager.GeneratePasswordResetTokenAsync(user);

        //    MailRequest mailRequest = new MailRequest()
        //    {
        //        ToEmail = email,
        //        Subject = $"Richiesta reset password",
        //        Body = $"Ci è pervenuta una richiesta di reset password! <br><br> Se non sei stato tu, ignora questo messaggio! " +
        //         $"<br><br> Se vuoi resettare la tua password <a href='https://wepp.art/resetpassword/" + token.Replace("/", "_").Replace("+", "&") + "/" + email + "'>clicca qui!</a>" +
        //         $"<br /> <br>" +
        //         $"<br /><br /> Team Weppart"
        //    };
        //    await _mailService.SendEmailAsync(mailRequest);

        //    return Ok();

        //}

        //[HttpPost]
        //[Route(nameof(ResetPassword))]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        //{
        //    try
        //    {
        //        var user = await userManager.FindByEmailAsync(model.email);
        //        var resetPassResult = await userManager.ResetPasswordAsync(user, model.token.Replace("_", "/").Replace("&", "+"), model.password);

        //        if (resetPassResult.Succeeded)
        //            return new OkObjectResult("Password Modificata");
        //        else
        //        {
        //            string error = string.Empty;
        //            if (resetPassResult.Errors.First().Code == "PasswordRequiresUpper")
        //                throw new NullReferenceException("La password deve contenere almeno una lettera maiuscola!");
        //            else if (resetPassResult.Errors.First().Code == "PasswordRequiresNonAlphanumeric")
        //                throw new NullReferenceException("La password deve contenere almeno un carattere speciale!");
        //            else if (resetPassResult.Errors.First().Code == "PasswordTooShort")
        //                throw new NullReferenceException("La password deve contenere almeno 8 caratteri!");
        //            else if (resetPassResult.Errors.First().Code == "PasswordRequiresDigit")
        //                throw new NullReferenceException("La password deve contenere almeno un numero!");
        //            else
        //                throw new Exception("Si è verificato un errore!");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is NullReferenceException)
        //            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });

        //        else
        //            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "Si è verificato un errore!" });
        //    }
        //}

        //[HttpPost]
        //[Route(nameof(UpdatePassword))]
        //public async Task<IActionResult> UpdatePassword([FromForm] ResetPasswordModel model)
        //{
        //    try
        //    {
        //        var user = await userManager.FindByEmailAsync(model.email);
        //        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        //        var resetPassResult = await userManager.ResetPasswordAsync(user, token, model.password);

        //        if (resetPassResult.Succeeded)
        //            return new OkObjectResult("Password Modificata");
        //        else
        //        {
        //            string error = string.Empty;
        //            if (resetPassResult.Errors.First().Code == "PasswordRequiresUpper")
        //                throw new NullReferenceException("La password deve contenere almeno una lettera maiuscola!");
        //            else if (resetPassResult.Errors.First().Code == "PasswordRequiresNonAlphanumeric")
        //                throw new NullReferenceException("La password deve contenere almeno un carattere speciale!");
        //            else if (resetPassResult.Errors.First().Code == "PasswordTooShort")
        //                throw new NullReferenceException("La password deve contenere almeno 8 caratteri!");
        //            else if (resetPassResult.Errors.First().Code == "PasswordRequiresDigit")
        //                throw new NullReferenceException("La password deve contenere almeno un numero!");
        //            else
        //                throw new Exception("Si è verificato un errore!");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is NullReferenceException)
        //            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = ex.Message });

        //        else
        //            return StatusCode(StatusCodes.Status500InternalServerError, new AuthResponseModel() { Status = "Error", Message = "Si è verificato un errore!" });
        //    }

        //}
    }
}
