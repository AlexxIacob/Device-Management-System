namespace backend.DTO
{
    //in the given task for register there is needed only email and passwords 
    //this dto is used after registration to update the users profile before he can actually do anything 
    public class UpdateProfileDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;


    }
}
