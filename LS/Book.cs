namespace LibrarySystem;

public class Book
{

    private string _isbn = ""; 
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN
    {
        get { return _isbn; }   // get method
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ISBN cannot be null or empty");
        
            if (!IsValidISBNFormat(value))
                throw new ArgumentException("Invalid ISBN format");
            
            _isbn = value;
        } 
    }
    private bool IsValidISBNFormat(string value)
    {
        // Validate that isbn does only contain numbers and hyphen.
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\d-]+$");
    }

    public int PublicationYear { get; set; }
    public bool IsBorrowed { get; set; }
    public DateTime? BorrowDate { get; set; }

    public Book(string title, string author, string isbn, int publicationYear)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        PublicationYear = publicationYear;
        IsBorrowed = false;
        BorrowDate = null;
    }
    
}