using LS;

namespace Advanced.NET_Labb4__UnitTestsMS;

[TestClass]
public class BookTest
{

    //- Varje bok måste ha ett ISBN-nummer
    [TestMethod]
    public void Book_BookIsCreatedWithISBN()
    {
        //Given: A new book is created with title, author, isbn and publication year
        string title = "Girl, Woman, Other";
        string author = "Bernadine Evaristo";
        string isbn = "9789100196400";
        int publicationYear = 2019;

        //When the book is created with a certain isbn nr (9780451524935)
        var book = new Book(title, author, isbn, publicationYear);

        //Then, the book should be successfully created and have that specific isbn nr (9780451524935). 
        Assert.AreEqual(isbn, book.ISBN);
    }

    [TestMethod]
    [DataRow( "")]
    [DataRow(null)]
    [DataRow("  ")]
    public void Book_NoSpecialCharacterOrWhiteSpacesInIsbn(string isbn)
    {
        //Given: 
        string title = "Gloria";
        string author = "Elisabeth Åsbring";
        int publicationYear = 2019;
        string incorrectISBN = isbn;

        // When & Then
        Assert.ThrowsException<ArgumentException>(() => 
            new Book(title, author, incorrectISBN, publicationYear));
    }
    
}