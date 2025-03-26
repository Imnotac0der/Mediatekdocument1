
namespace MediaTekDocuments.model
{
    using System;
    /// <summary>
    /// Classe métier Utilisateur
    /// </summary>
    public class Utilisateur
    {
        public int Id { get; }
        public string Firstname { get; }
        public string Name { get; }
        public string Login { get; }
        public string Password { get; }
        public string IdService { get; }

        public Utilisateur(int id, string firstname, string name, string login, string password, string idService )
        {
            this.Id = id;
            this.Firstname = firstname;
            this.Name = name;
            this.Login = login;
            this.Password = password;
            this.IdService = idService;
        }

    }
}
