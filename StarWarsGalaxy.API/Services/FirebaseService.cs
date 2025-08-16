using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using System.Security.Cryptography.X509Certificates;

namespace StarWarsGalaxy.API.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseService()
        {
            try
            {

                string[] possiblePaths = {
                    Path.Combine(Directory.GetCurrentDirectory(), "Config", "test-proyecto-fc060-firebase-adminsdk-fbsvc-f2d463336a.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "test-proyecto-fc060-firebase-adminsdk-fbsvc-f2d463336a.json"),
                    "Config/test-proyecto-fc060-firebase-adminsdk-fbsvc-f2d463336a.json"
                };

                string credentialPath = "";
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        credentialPath = path;
                        Console.WriteLine($"Archivo encontrado en: {path}");
                    }
                }

                if (string.IsNullOrEmpty(credentialPath))
                {
                    throw new FileNotFoundException("Archivo no encontrado");
                }

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

                var credential = GoogleCredential.FromFile(credentialPath);

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = credential,
                        ProjectId = "test-proyecto-fc060"
                    });
                }

                _firestoreDb = new FirestoreDbBuilder
                {
                    ProjectId = "test-proyecto-fc060",
                    Credential = credential
                }.Build();

                _firebaseAuth = FirebaseAuth.DefaultInstance;

                Console.WriteLine("Firebase inicializado correctamente.");

            }catch (Exception ex){ 
                Console.WriteLine($"Error inicializando Firebase: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        //Autenticacion
        public async Task<UserRecord> CreateUserAsync(string email, string password)
        {
            var userRecordArgs = new UserRecordArgs()
            {
                Email = email,
                Password = password,
                EmailVerified = false,
                Disabled = false
            };
            return await _firebaseAuth.CreateUserAsync(userRecordArgs);
        }

        public async Task<string?> VerifyTokenAsync(string idToken)
        {
            try
            {
                var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
                return decodedToken.Uid;
            }
            catch
            {
                return null;
            }
        }

        //Guardar el perfil
        public async Task SaveUserProfileAsync(string userId, object userData)
        {
            var docRef = _firestoreDb.Collection("users").Document(userId);
            await docRef.SetAsync(userData);
        }

        public async Task<T?> GetUserProfileAsync<T>(string userId) where T : class
        {
            var docRef = _firestoreDb.Collection("users").Document(userId);
            var snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<T>() : null;
        }
    }
}
