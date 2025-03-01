﻿using Integration_Test.Extensions;
using Integration_Test.V1.Exceptions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System.Net;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.Session
{
    [TestClass]
    public class SessionTest
    {
        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly SessionLib _sessionLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly RestEmulatorLib _emulatorLib;

        public SessionTest()
        {
            _sessionLib = new SessionLib(baseUri);
            _userLib = new UserLib(baseUri);
            _cleanUpLib = new CleanUpLib(baseUri);
            _emulatorLib = new RestEmulatorLib(emulatorUri);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task TestCreateDefaultSession()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();
            Guid userId = user["userId"].Value<Guid>();

            string title = "Session Title";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());

            // Act
            HttpResponseMessage response = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            response.EnsureSuccessStatusCode();

            // Assert
            string rawResponse = await response.Content.ReadAsStringAsync();
            JsonObject session = JsonSerializer.Deserialize<JsonObject>(rawResponse);

            Assert.IsTrue(session.ContainsKey("id"));
            Assert.AreEqual(sportType, session["sportType"].Value<string>());
            Assert.AreEqual(userId.ToString(), session["creatorId"].Value<Guid>().ToString());
            Assert.AreEqual(userId.ToString(), session["participants"].AsArray()[0].ToString());

            Assert.AreEqual(title, session["title"].Value<string>());
            Assert.AreEqual(description, session["description"].Value<string>());
            Assert.AreEqual("Everswinkel", session["location"].AsObject()["city"].Value<string>());
            Assert.AreEqual("48351", session["location"].AsObject()["zipCode"].Value<string>());
            Assert.AreEqual(date, session["date"].Value<DateTime>());
            Assert.AreEqual(minParticipants, session["minParticipants"].Value<int>());
            Assert.AreEqual(maxParticipants, session["maxParticipants"].Value<int>());
            Assert.AreEqual(string.Join(' ', tags), string.Join(' ', session["tags"].AsArray().Select(x => x.Value<string>()).ToList()));
            Assert.AreEqual(1, session["participants"].AsArray().Count);
        }

        [TestMethod]
        public async Task TestCreateSessionWithoutTitle()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            string title = "";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());


            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestCreateSessionInvalidDescription()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            string title = "Test Title";
            string sportType = "Basketball";
            string description = string.Join(' ', Enumerable.Range(0, 1000).Select(x => "x")); //Too long
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());


            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestCreateSessionInvalidSportType()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            string title = "TestTitle";
            string sportType = "coolAsSportType";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());


            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        [DataRow(9999, 7.846992)]
        [DataRow(51.924470, -9999)]
        [DataRow(-91, 0)]
        [DataRow(0, 181)]
        public async Task TestCreateSessionWithInvalidCoordinates(double latitude, double longitude)
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(
                accessToken: token,
                title: "TestTitle",
                sportType: "Basketball",
                description: "Session Description",
                latitude: latitude,
                longitude: longitude,
                date: DateTime.UtcNow.AddDays(1),
                minParticipants: 5,
                maxParticipants: 10,
                tags: ["tag1", "tag2"]
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestCreateSessionWithInvalidParticipantLimits()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            string title = "TestTitle";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(1);
            int minParticipants = 10;
            int maxParticipants = 5; // Invalid, then min > max
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestCreateSessionWithInvalidTags()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            string title = "TestTitle";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = Enumerable.Range(0, 100).Select(i => "Example Text").ToList(); // Many Tags

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestCreateSessionWithPastDate()
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            string title = "TestTitle";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.UtcNow.AddDays(-1); // Date in the past
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestJoinSession()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            Guid sessionId = session["id"].Value<Guid>();

            JsonObject joinUser = await _userLib.CreateDefaultUser(true);
            string joinUserToken = joinUser["accessToken"].Value<string>();
            Guid joinUserId = joinUser["userId"].Value<Guid>();

            // Act
            HttpResponseMessage response = await _sessionLib.JoinSession(sessionId, joinUserToken);
            response.EnsureSuccessStatusCode();

            // Assert
            JsonObject sessionAfterJoin = await _sessionLib.GetSession(sessionId, createUserToken);
            Assert.AreEqual(2, sessionAfterJoin["participants"].AsArray().Count);
            Assert.IsTrue(sessionAfterJoin["participants"].AsArray().Any(x => x.Value<Guid>() == joinUserId));
        }

        [TestMethod]
        public async Task TestJoinNonExistentSession()
        {
            // Arrange
            JsonObject joinUser = await _userLib.CreateDefaultUser(true);
            string joinUserToken = joinUser["accessToken"].Value<string>();
            Guid nonExistentSessionId = Guid.NewGuid();

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.JoinSession(nonExistentSessionId, joinUserToken);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestLeaveSession()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            Guid sessionId = session["id"].Value<Guid>();

            JsonObject leaveUser = await _userLib.CreateDefaultUser(true);
            string leaveUserToken = leaveUser["accessToken"].Value<string>();
            Guid leaveUserId = leaveUser["userId"].Value<Guid>();

            HttpResponseMessage joinResponse = await _sessionLib.JoinSession(sessionId, leaveUserToken);
            joinResponse.EnsureSuccessStatusCode();

            // Act
            HttpResponseMessage leaveResponse = await _sessionLib.LeaveSession(sessionId, leaveUserToken);
            leaveResponse.EnsureSuccessStatusCode();

            // Assert
            JsonObject sessionAfterLeave = await _sessionLib.GetSession(sessionId, createUserToken);
            Assert.AreEqual(1, sessionAfterLeave["participants"].AsArray().Count);
            Assert.IsFalse(sessionAfterLeave["participants"].AsArray().Any(x => x.Value<Guid>() == leaveUserId));
        }

        [TestMethod]
        public async Task TestLeaveNonExistentSession()
        {
            // Arrange
            JsonObject leaveUser = await _userLib.CreateDefaultUser(true);
            string leaveUserToken = leaveUser["accessToken"].Value<string>();

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.LeaveSession(Guid.NewGuid(), leaveUserToken);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestKickUserFromSession()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            Guid sessionId = session["id"].Value<Guid>();

            JsonObject kickUser = await _userLib.CreateDefaultUser(true);
            string kickUserToken = kickUser["accessToken"].Value<string>();
            Guid kickUserId = kickUser["userId"].Value<Guid>();

            HttpResponseMessage joinResponse = await _sessionLib.JoinSession(sessionId, kickUserToken);
            joinResponse.EnsureSuccessStatusCode();

            // Act
            HttpResponseMessage kickResponse = await _sessionLib.KickUserFromSession(sessionId, kickUserId, createUserToken);
            kickResponse.EnsureSuccessStatusCode();

            // Assert
            JsonObject sessionAfterKick = await _sessionLib.GetSession(sessionId, createUserToken);
            Assert.AreEqual(1, sessionAfterKick["participants"].AsArray().Count);
            Assert.IsFalse(sessionAfterKick["participants"].AsArray().Any(x => x.Value<Guid>() == kickUserId));
        }

        [TestMethod]
        public async Task TestKickUserFromNonExistentSession()
        {
            // Arrange
            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();
            Guid nonExistentSessionId = Guid.NewGuid();
            Guid kickUserId = Guid.NewGuid();

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.KickUserFromSession(nonExistentSessionId, kickUserId, createUserToken);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestDeleteSession()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            Guid sessionId = session["id"].Value<Guid>();

            // Act
            HttpResponseMessage deleteResponseMessage = await _sessionLib.DeleteSession(sessionId, createUserToken);
            deleteResponseMessage.EnsureSuccessStatusCode();

            // Assert
            await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await _sessionLib.GetSession(sessionId, createUserToken));
        }

        [TestMethod]
        public async Task TestDeleteNonExistentSession()
        {
            // Arrange
            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();
            Guid nonExistentSessionId = Guid.NewGuid();

            // Act
            HttpResponseMessage responseMessage = await _sessionLib.DeleteSession(nonExistentSessionId, createUserToken);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestGetSession()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            Guid sessionId = session["id"].Value<Guid>();

            JsonObject getUser = await _userLib.CreateDefaultUser(true);
            string getUserToken = getUser["accessToken"].Value<string>();

            // Act
            JsonObject getUserResponse = await _sessionLib.GetSession(sessionId, getUserToken);
            JsonObject getCreateUserResponse = await _sessionLib.GetSession(sessionId, createUserToken);

            // Assert
            Assert.IsNull(getUserResponse["participants"]);
            Assert.IsNotNull(getCreateUserResponse["participants"]);
        }

        [TestMethod]
        public async Task TestGetSessionsFromUser()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            // Act & Assert
            JsonArray sessions = await _sessionLib.GetSesssionFromUser(createUserToken);
            Assert.AreEqual(0, sessions.Count);

            // Act & Assert
            await _sessionLib.CreateDefaultSession(createUserToken);
            sessions = await _sessionLib.GetSesssionFromUser(createUserToken);
            Assert.AreEqual(1, sessions.Count);
        }

        [TestMethod]
        public async Task TestGetSessionsFromUser_JoinedSession()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject joinUser = await _userLib.CreateDefaultUser(true);
            string joinUserToken = joinUser["accessToken"].Value<string>();

            // Act & Assert
            JsonArray sessions = await _sessionLib.GetSesssionFromUser(createUserToken);
            Assert.AreEqual(0, sessions.Count);

            // Act & Assert
            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            sessions = await _sessionLib.GetSesssionFromUser(createUserToken);
            Assert.AreEqual(1, sessions.Count, "Creator should have created a session");

            // Act & Assert
            Guid sessionId = session["id"].Value<Guid>();
            await _sessionLib.JoinSession(sessionId, joinUserToken);
            sessions = await _sessionLib.GetSesssionFromUser(joinUserToken);
            Assert.AreEqual(1, sessions.Count, "Joiner should have created a session");
        }

        [TestMethod]
        public async Task TestGetSessionsFromUser_JoinedSession_DeleteAccount()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            JsonObject joinUser = await _userLib.CreateDefaultUser(true);
            string joinUserToken = joinUser["accessToken"].Value<string>();

            // Act & Assert
            JsonArray sessions = await _sessionLib.GetSesssionFromUser(createUserToken);
            Assert.AreEqual(0, sessions.Count);

            // Act & Assert
            JsonObject session = await _sessionLib.CreateDefaultSession(createUserToken);
            sessions = await _sessionLib.GetSesssionFromUser(createUserToken);
            Assert.AreEqual(1, sessions.Count, "Creator should have created a session");

            // Act
            Guid sessionId = session["id"].Value<Guid>();
            await _sessionLib.JoinSession(sessionId, joinUserToken);
            await _userLib.DeleteUser(joinUserToken);

            // Assert
            session = await _sessionLib.GetSession(sessionId, createUserToken);
            Assert.AreEqual(1, session["participants"].AsArray().Count);
        }

    }
}
