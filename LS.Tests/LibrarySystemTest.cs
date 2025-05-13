using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net.Security;
using LS;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace Advanced.NET_Labb4__UnitTestsMS;

[TestClass]
public class LibrarySystemTest
{
    private LibrarySystem _librarySystem;

    public LibrarySystemTest()
    {
        _librarySystem = new LibrarySystem();
    }

    public Book AddTestBook()
    {
        var book = new Book("Girl, Woman, Other", "Bernadine Evaristo", "9789100196400", 2019);
        _librarySystem.AddBook(book);

        return book; 
    }

    //- Det får inte finnas dubletter
    [TestMethod]
    public void DuplicatesOfIsbnNotAllowed()
    {
        //Given that there a book is already in the system
        AddTestBook();
        var book = new Book("Girl, Woman, Other", "Bernadine Evaristo", "9789100196400", 2019);

        //When the same book is added again,
        //Then an error should be throuwn and the book is not added.
        Assert.ThrowsException<ArgumentException>(() =>
            _librarySystem.AddBook(book));
    }
    
    //böcker ska kunna tas bort ut systemet. 
    [TestMethod]
    public void BookCanBeRemoved()
    {
        //Given a book object with 
        var book = AddTestBook();
        
        //When this object is removed 
        bool removalResult = _librarySystem.RemoveBook(book.ISBN);
        var searchResult = _librarySystem.SearchByISBN(book.ISBN); 

        //Then: 
        Assert.IsTrue(removalResult, "Removebook should return true when successful");
        Assert.IsNull(searchResult, "SearchByIsbn shoudl retutn null when book has been removed");
    }
    
    //Böcker som är utlånade ska inte kunna tas bort från systemet
    [TestMethod]
    public void BorrowedBooksCannotBeremoved()
    {
        //Given a book is borrowed.
        var book = AddTestBook(); 
        _librarySystem.BorrowBook(book.ISBN); 

        //When a attempt to remove this book is done
        bool removalResult = _librarySystem.RemoveBook(book.ISBN);
        
        //The book should no the removed.
        Assert.IsFalse(removalResult, "RemoveBook should return false when a book is borowed.");
    }
    
    //Sökning ska vara skiftlägesokänslig (versaler/gemener ska ge samma resultat)
    [DataRow ("Girl, Woman, Other")]
    [DataRow ("GIRL, WOMAN, OTHER")]
    [DataRow ("girl, woman, other")]
    [TestMethod]
    public void CaseInSensitiveTitleSearch(string title)
    {
        //Given a book that is added to the system
        var book = AddTestBook(); 

        //When searching for the same titel, using different combinations of cases (Capital and small)
        var searchResult = _librarySystem.SearchByTitle(title);
        
        //Then the book should still be found (the list of books should not be empty and the isbn should
        //match).
        Assert.IsTrue(searchResult.Count > 0, "SearchResult should not be empty");
        Assert.IsTrue(searchResult.Any(b => b.ISBN == book.ISBN));
    }
    
    //Sökning ska vara skiftlägesokänslig (versaler/gemener ska ge samma resultat)
    [DataRow ("Bernadine Evaristo")]
    [DataRow ("BERNADINE EVARISTO")]
    [DataRow ("bernadine evaristo")]
    [TestMethod]
    public void CaseInSensitiveAuthorSearch(string author)
    {
        //Given a book that is added to the system
        var book = AddTestBook(); 

        //When searching for the same titel, using different combinations of cases (Capital and small)
        var searchResult = _librarySystem.SearchByAuthor(author);
        
        //Then the book should still be found (the list of books should not be empty and the isbn should
        //match).
        Assert.IsTrue(searchResult.Count > 0, "SearchResult should not be empty");
        Assert.IsTrue(searchResult.Any(b => b.ISBN == book.ISBN));
    }
    
    //Sökningar ska kunna hitta böcker på delmatchningar (inte bara exakta matchningar)
    [DataRow ("Girl")]
    [DataRow ("WOM")]
    [DataRow ("other")]
    [TestMethod]
    public void SearchPossibleOnPartofTitle(string partOfTitle)
    {
        //Given a book that is added to the system
        var book = AddTestBook(); 
    
        //When searching for the same titel, using different combinations of cases (Capital and small)
        var searchResult = _librarySystem.SearchByTitle(partOfTitle);
         
        //Then the book should still be found (the list of books should not be empty and the isbn should
        //match).
        Assert.IsTrue(searchResult.Count > 0, "SearchResult should not be empty");
        Assert.IsTrue(searchResult.Any(b => b.ISBN == book.ISBN));
    }
     
    //Sökningar ska kunna hitta böcker på delmatchningar (inte bara exakta matchningar)
    [DataRow ("Bern")]
    [DataRow ("Evar")]
    [DataRow ("isto")]
    [TestMethod]
    public void SearchPossibleOnPartofAuthor(string partOfAuthor)
    {
        //Given a book that is added to the system
        var book = AddTestBook(); 
    
        //When searching for the same titel, using different combinations of cases (Capital and small)
        var searchResult = _librarySystem.SearchByAuthor(partOfAuthor);
         
        //Then the book should still be found (the list of books should not be empty and the isbn should
        //match).
        Assert.IsTrue(searchResult.Count > 0, "SearchResult should not be empty");
        Assert.IsTrue(searchResult.Any(b => b.ISBN == book.ISBN));
    }
    
    //Test to see what happens if no match
    [TestMethod]
    public void NonMatchingTitleQuery()
    {
        //Given: 
        AddTestBook();
     
        //When
        var searchResult = _librarySystem.SearchByTitle("Nonexistent Title");
     
        //Then:
        Assert.IsNotNull(searchResult);
        Assert.AreEqual(0, searchResult.Count);
    }
     
    //En bok som lånas ut ska markeras som utlånad i systemet
    [TestMethod]
    public void BorroweBookMarkedCorrectly()
    {
        //Given
        var book = AddTestBook();
         
        //When
        _librarySystem.BorrowBook(book.ISBN);
         
        //Then
        Assert.IsTrue(book.IsBorrowed);
    }
     
    //Redan utlånade böcker ska inte kunna lånas ut
    [TestMethod]
    public void BorroweBookCannotBeBorrowedAgain()
    {
        //Given
        var book = AddTestBook();
        _librarySystem.BorrowBook(book.ISBN);
         
        //When someone wants to borrow book again
        bool borrowBookresult = _librarySystem.BorrowBook(book.ISBN); 
         
        //Then
        Assert.IsFalse(borrowBookresult, "BorrowedBook should be false if book is already borrowed");
    }
     
    //När en bok lånas ska rätt utlåningsdatum sättas
    [TestMethod]
    public void CorrectDateIsSetWhenBookIsBorrowed()
    {
        //Given:
        var book = AddTestBook();
             
        //When:
        _librarySystem.BorrowBook(book.ISBN);
             
        //Then
        var today = DateTime.Today; 
        Assert.IsNotNull(book.BorrowDate, "Borrowed date should be set");
        Assert.AreEqual(today, book.BorrowDate.Value.Date);
    }
    
    //Vid återlämning ska bokens utlåningsdatum nollställas
    [TestMethod]
    public void BorrowedDateIsResetWhenReturned()
    {
        //Given:
        var book = AddTestBook();
        _librarySystem.BorrowBook(book.ISBN);

        //When
        _librarySystem.ReturnBook(book.ISBN); 

        //Then: 
        Assert.IsFalse(book.IsBorrowed);
        Assert.IsNull(book.BorrowDate);
    }
    
    //Endast utlånade böcker ska kunna återlämnas
    [TestMethod]
    public void BooksThatAreNotBorrowedCannotBeReturned()
    {
        //Given:
        var book = AddTestBook();

        //When
        var returnBookResult = _librarySystem.ReturnBook(book.ISBN); 

        //Then: 
        Assert.IsFalse(returnBookResult);
    }
    
    //Korrekt beräkning av om en bok är försenad ska implementeras
    public void OverdueBookReturnsTrue()
    {
        // Given a book that was borrowed 15days ago
        var book = AddTestBook();
        book.IsBorrowed = true;
        book.BorrowDate = DateTime.Now.AddDays(-15);

        int loanPeriod = 14; // t.ex. 2 veckor

        // When
        bool isOverdue = _librarySystem.IsBookOverdue(book.ISBN, loanPeriod);

        // Then
        Assert.IsTrue(isOverdue, "Book should be marked as overdue.");
    }

    [TestMethod]
    public void BookNotOverdueWithinLoanPeriod()
    {
        // Given: a book that was borrowed 10 days ago.
        var book = AddTestBook();
        book.IsBorrowed = true;
        book.BorrowDate = DateTime.Now.AddDays(-10);

        int loanPeriod = 14;

        // When
        bool isOverdue = _librarySystem.IsBookOverdue(book.ISBN, loanPeriod);

        // Then
        Assert.IsFalse(isOverdue, "Book should not be marked as overdue.");
    }

    [TestMethod]
    public void BookNotOverdueWhenNotBorrowed()
    {
        // Given a book that os not borrowed
        var book = AddTestBook();

        int loanPeriod = 14;

        // When
        bool isOverdue = _librarySystem.IsBookOverdue(book.ISBN, loanPeriod);

        // Then
        Assert.IsFalse(isOverdue, "Unborrowed book should not be overdue.");
    }

}