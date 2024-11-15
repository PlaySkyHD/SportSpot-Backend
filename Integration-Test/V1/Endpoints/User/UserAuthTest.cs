﻿using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using System.Net;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.User
{
    [TestClass]
    public class UserAuthTest
    {

        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly UserLib _userLib;
        private readonly CleanUpLib _cleanUpLib;

        public UserAuthTest()
        {
            _userLib = new(baseUri);
            _cleanUpLib = new(baseUri);
        }


        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task RegisterUserWithoutImage()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            ValidateToken(authToken);
        }

        [TestMethod]
        public async Task RegisterUserWithImage()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = _userLib.GetDefaultPictureAsBase64();
            
            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            ValidateToken(authToken);
        }

        [TestMethod]
        public async Task RegisterUserWithInvalidImageWithRetry()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = "ssasaa1sas";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent);
            
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Invalid Base64 string", errorInformation[0].AsObject()["Message"].Value<string>());

            // Retry

            // Arrange
            avatar = _userLib.GetDefaultPictureAsBase64();

            // Act
            response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);

            responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            ValidateToken(authToken);
        }

        [TestMethod]
        public async Task RegisterMultipleUser()
        {
            // Arrange

            //User One
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = _userLib.GetDefaultPictureAsBase64();

            //User Two
            string username2 = "TestUser2";
            string email2 = "max.musterman2@gmail.com";
            string password2 = "password1.G.222";
            string firstname2 = "Max";
            string lastname2 = "Musterman";

            string avatar2 = _userLib.GetDefaultPictureAsBase64();

            // Act

            //User One
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            //User Two
            HttpResponseMessage response2 = await _userLib.RegisterUser(username2, email2, password2, firstname2, lastname2, avatar2);
            response2.EnsureSuccessStatusCode();

            string responseContent2 = await response2.Content.ReadAsStringAsync();
            JsonObject authToken2 = JsonSerializer.Deserialize<JsonObject>(responseContent2);

            // Assert
            ValidateToken(authToken);
            ValidateToken(authToken2);
        }

        [TestMethod]
        public async Task RegisterUserInvalidPassword()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "p";
            string firstname = "Max";
            string lastname = "Musterman";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User.PasswordTooShort", errorInformation[0].AsObject()["Code"].Value<string>());
            Assert.AreEqual("User.PasswordRequiresNonAlphanumeric", errorInformation[1].AsObject()["Code"].Value<string>());
            Assert.AreEqual("User.PasswordRequiresDigit", errorInformation[2].AsObject()["Code"].Value<string>());
            Assert.AreEqual("User.PasswordRequiresUpper", errorInformation[3].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task RegisterUserInvalidMail()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.mustermangmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task RegisterUserEmptyRequest()
        {
            // Arrange
            string username = "";
            string email = "";
            string password = "";
            string firstname = "";
            string lastname = "";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task RegisterMultipleUserSameMail()
        {
            // Arrange

            //User One
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = _userLib.GetDefaultPictureAsBase64();

            //User Two
            string username2 = "TestUser2";
            string email2 = "max.musterman@gmail.com";
            string password2 = "password1.G.222";
            string firstname2 = "Max";
            string lastname2 = "Musterman";

            string avatar2 = _userLib.GetDefaultPictureAsBase64();

            // Act
            //User One
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            //User Two
            HttpResponseMessage response2 = await _userLib.RegisterUser(username2, email2, password2, firstname2, lastname2, avatar2);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
            string responseContent2 = await response2.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent2);

            // Assert
            ValidateToken(authToken);
            Assert.AreEqual("User.DuplicateEmail", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task RegisterMultipleUserSameUsername()
        {
            // Arrange

            //User One
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = _userLib.GetDefaultPictureAsBase64();

            //User Two
            string username2 = "TestUser";
            string email2 = "max.musterman2@gmail.com";
            string password2 = "password1.G.222";
            string firstname2 = "Max";
            string lastname2 = "Musterman";

            string avatar2 = _userLib.GetDefaultPictureAsBase64();

            // Act
            //User One
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            //User Two
            HttpResponseMessage response2 = await _userLib.RegisterUser(username2, email2, password2, firstname2, lastname2, avatar2);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
            string responseContent2 = await response2.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent2);

            // Assert
            ValidateToken(authToken);
            Assert.AreEqual("User.DuplicateUserName", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task LoginUserWithUsernameAndMail()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";
            string avatar = _userLib.GetDefaultPictureAsBase64();

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            // Act Login with Username
            HttpResponseMessage loginResponse = await _userLib.LoginUser(username, password);

            loginResponse.EnsureSuccessStatusCode();

            string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            JsonObject loginAuthToken = JsonSerializer.Deserialize<JsonObject>(loginResponseContent);

            // Assert
            ValidateToken(loginAuthToken);

            // Act Login with Mail
            HttpResponseMessage loginResponse2 = await _userLib.LoginUser(email, password);

            loginResponse2.EnsureSuccessStatusCode();

            string loginResponseContent2 = await loginResponse.Content.ReadAsStringAsync();
            JsonObject loginAuthToken2 = JsonSerializer.Deserialize<JsonObject>(loginResponseContent2);

            // Assert
            ValidateToken(loginAuthToken2);
        }

        [TestMethod]
        public async Task LoginUserInvalidPassword()
        {
            // Arrange User
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";
            string avatar = _userLib.GetDefaultPictureAsBase64();

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            // Act Login with wrong Password
            HttpResponseMessage loginResponse = await _userLib.LoginUser(username, password + "fake");

            string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(loginResponseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
            Assert.AreEqual("User.Login", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task LoginUserInvalidUsername()
        {
            // Arrange User
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";
            string avatar = _userLib.GetDefaultPictureAsBase64();

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            // Act Login with wrong Password
            HttpResponseMessage loginResponse = await _userLib.LoginUser(username + "fake", password);

            string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(loginResponseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
            Assert.AreEqual("User.Login", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        private static void ValidateToken(JsonObject authToken)
        {
            Assert.IsNotNull(authToken, "Authentication token should not be null.");
            Assert.IsFalse(string.IsNullOrEmpty(authToken["accessToken"].Value<string>()), "Access token should not be empty or null.");
            Assert.IsTrue(authToken["accessExpire"].Value<DateTime>() > DateTime.Now, "Access token should expire in the future.");
            Assert.IsFalse(string.IsNullOrEmpty(authToken["refreshToken"].Value<string>()), "Refresh token should not be empty or null.");
            Assert.IsTrue(authToken["refreshExpire"].Value<DateTime>() > DateTime.Now, "Refresh token should expire in the future.");
        }

    }
}