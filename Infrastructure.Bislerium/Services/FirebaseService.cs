using Application.Bislerium;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUserService _userServices;
        private static readonly object _lock = new object();
        private static bool _firebaseInitialized = false;

        public FirebaseService(ApplicationDBContext dbContext, IUserService userServices)
        {
            this._dbContext = dbContext;
            this._userServices = userServices;

            InitializeFirebaseApp();
        }

        private void InitializeFirebaseApp()
        {
            lock (_lock)
            {
                if (!_firebaseInitialized && FirebaseApp.DefaultInstance == null)
                {
                    // Get the base directory of the current application domain
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    // Combine the base directory with the relative path to the credentials file
                    string credentialsPath = Path.Combine(basePath, "..", "..", "..", "FirebaseCredentials", "firebase-adminsdk.json");

                    // Initialize Firebase Admin SDK with the credentials file
                    var firebaseCredential = GoogleCredential.FromFile(credentialsPath);

                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = firebaseCredential
                    });

                    _firebaseInitialized = true;
                }
            }
        }

        // Save Firebase Token
        public async Task<FirebaseToken> CreateNewToken(CreateToken payload)
        {
            User? user = await _userServices.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Token cannot be saved before login.");

            FirebaseToken newToken; // Declare newToken outside the if-else block

            // Check if the user already has a token
            FirebaseToken existingToken = await _dbContext.FirebaseTokens.FirstOrDefaultAsync(t => t.UserID == payload.UserID);

            if (existingToken != null)
            {
                // If the token already exists for the user, update it
                existingToken.Token = payload.Token;
            }
            else
            {
                // If the token doesn't exist for the user, create a new one
                newToken = new FirebaseToken
                {
                    Token = payload.Token,
                    UserID = payload.UserID,
                };
                await _dbContext.FirebaseTokens.AddAsync(newToken);
            }

            await _dbContext.SaveChangesAsync();

            return existingToken;
        }

        // Send push notification
        public async Task SendPushNotifications(IEnumerable<string> userIds, string title, string body)
        {
            Console.WriteLine("La gayo");

            // Convert the user IDs from string to Guid
            var userIdsGuid = userIds.Select(id => Guid.Parse(id));

            // Fetch tokens for the given user IDs
            var tokens = await _dbContext.FirebaseTokens
                .Where(token => userIdsGuid.Contains(token.UserID) && token.Token != null)
                .Select(token => token.Token)
                .ToListAsync();

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
                // Log or handle the response as needed
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

    }
}
