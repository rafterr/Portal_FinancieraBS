using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;

namespace FinancieraBS.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly string _apiKey;
        private readonly string _bucket;
        private readonly string _authEmail;
        private readonly string _authPassword;

        public FirebaseStorageService(IConfiguration configuration)
        {
            _apiKey = configuration["Firebase:ApiKey"] ?? "";
            _bucket = configuration["Firebase:Bucket"] ?? "";
            _authEmail = configuration["Firebase:AuthEmail"] ?? "";
            _authPassword = configuration["Firebase:AuthPassword"] ?? "";
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder, string fileName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            var authConfig = new FirebaseAuthConfig
            {
                ApiKey = _apiKey,
                AuthDomain = $"{_bucket}",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            var auth = new FirebaseAuthClient(authConfig);
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

            using var stream = file.OpenReadStream();
            var task = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken),
                    ThrowOnCancel = true
                })
                .Child(folder)
                .Child(fileName)
                .PutAsync(stream);

            return await task;
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl)) return false;

                var authConfig = new FirebaseAuthConfig
                {
                    ApiKey = _apiKey,
                    AuthDomain = $"{_bucket}",
                    Providers = new FirebaseAuthProvider[]
                    {
                        new EmailProvider()
                    }
                };

                var auth = new FirebaseAuthClient(authConfig);
                var userCredential = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

                var storage = new FirebaseStorage(
                    _bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken)
                    });

                await storage.Child(fileUrl).DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetFileUrlAsync(string filePath)
        {
            var authConfig = new FirebaseAuthConfig
            {
                ApiKey = _apiKey,
                AuthDomain = $"{_bucket}",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            var auth = new FirebaseAuthClient(authConfig);
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

            var storage = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken)
                });

            return await storage.Child(filePath).GetDownloadUrlAsync();
        }
    }
}
