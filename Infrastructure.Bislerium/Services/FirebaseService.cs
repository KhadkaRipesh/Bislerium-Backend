using Application.Bislerium;
using Domain.Bislerium.DTOs.Notification;
using Domain.Bislerium.Exceptions;
using Domain.Bislerium.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Bislerium.Services
{
    public class FirebaseService: IFirebaseService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IUserService userServices;
        private static bool _firebaseInitialized = false;

        public FirebaseService(ApplicationDBContext dbContext, IUserService userServices)
        {
            this._dbContext = dbContext;
            this.userServices = userServices;

            if(!_firebaseInitialized )
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

        // Save Firebase Token
        public async Task<FirebaseToken> CreateNewToken(CreateToken payload)
        {
            User? user = await userServices.GetCurrentUser();
            if (user == null)
                throw new ProgramException("Token cannot be saved before login.");
            FirebaseToken token = new FirebaseToken
            {
                Token = payload.Token,
                UserID = payload.UserID,
            };
            await _dbContext.FirebaseTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return token;
        } 
    }
}
