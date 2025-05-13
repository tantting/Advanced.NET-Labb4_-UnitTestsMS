using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using LibrarySystem;

namespace Advanced.NET_Labb4__UnitTestsMS;

[TestClass]
public class LibrarySystemTest
{
    private LibrarySystem.LibrarySystem _librarySystem;

    public LibrarySystemTest()
    {
        _librarySystem = new LibrarySystem.LibrarySystem();
    }
}

    //- Det får inte finnas dubletter
    [TestMethod]
    public void DuplicatesOfIsbnNotAllowed()
    {
        //Given: a new book is created
        //Given: 
        var book = new Book("1984", "George Orwell", "9780451524935", 1949);

        //When: the new book is to be added to the list of books
        //Then: it should be validated that a book with the same isbn-nr is not already in the list, and if so, 
        //an error should be throuwn and the book is not added.
        Assert.ThrowsException<ArgumentException>(() => 

    }
}