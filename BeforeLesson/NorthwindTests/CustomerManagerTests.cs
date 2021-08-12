using NUnit.Framework;
using NorthwindBusiness;
using NorthwindData;
using System.Linq;

namespace NorthwindTests
{
    public class CustomerTests
    {
        CustomerManager _customerManager;
        [SetUp]
        public void Setup()
        {
            _customerManager = new CustomerManager();
            // remove test entry in DB if present
            using (var db = new NorthwindContext())
            {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "MANDA"
                select c;

                db.Customers.RemoveRange(selectedCustomers);
                db.SaveChanges();
            }
        }

        [Test]
        public void WhenANewCustomerIsAdded_TheNumberOfCustomersIncreasesBy1()
        {
            
            _customerManager = new CustomerManager();
            int beforeAddCount = _customerManager.RetrieveAllCustomers().Count();
            int countAfterAddingNewCustomer;
            _customerManager.Create("TEST", "Temp Entry", "McTemp", "Rivendale");       //Add customer
            using (var db = new NorthwindContext())
            {
                //check count
                countAfterAddingNewCustomer =
                    db.Customers.Count();
                //Remove temp customer entry
                var tempCustomer = db.Customers.Where(c => c.CustomerId == "TEST");
                db.RemoveRange(tempCustomer);
                db.SaveChanges();
                
            }
            Assert.That(beforeAddCount, Is.EqualTo(countAfterAddingNewCustomer - 1));

        }

        [TestCase("TEST1", "Bob TheBuilder", "McDonalds", "London")]
        [TestCase("TEST2", "Bilbo Baggins", "Dwarf Company", "The Shire")]
        [TestCase("TEST3", "Hagrid", "Hogwarts", "Hogwarts")]
        [TestCase("TEST4", "Henry Ford", "Ford CO.", "Detroit")]
        public void WhenANewCustomerIsAdded_TheirDetailsAreCorrect(string customerID, string name, string company, string city)
        {
            _customerManager.Delete(customerID);    //remove if in dataBase
            string expectedResult = $"{customerID} {name} {company} {city}";
            _customerManager.Create(customerID, name, company, city);
            string result = ""; ;
            using (var db = new NorthwindContext())
            {
                var tempCustomer =
                    db.Customers.Where(c => c.CustomerId == customerID).FirstOrDefault();

                result += $"{tempCustomer.CustomerId} " +
                    $"{tempCustomer.ContactName} " +
                    $"{tempCustomer.CompanyName} " +
                    $"{tempCustomer.City}";
                
                //Remove temp customer entry
                db.RemoveRange(tempCustomer);
                db.SaveChanges();

            }


            Assert.That(result, Is.EqualTo(expectedResult));

        }

        [Test]
        public void WhenACustomerIsUpdated_TheDatabaseIsUpdated()
        {
            _customerManager.Create("TEST1", "Henry Ford", "Ford CO.", "Detroit");  //create temp entry
            _customerManager.Update("TEST1", "Henry Ford", "Ford CO.", "New York", "BOX101");
            string resultPostalCode = "";
            string resultCityUpdate = "";
            using (var db = new NorthwindContext())
            {
                var queryTempCustUpdate =
                    db.Customers.Where(c => c.CustomerId == "TEST1").FirstOrDefault();
                resultCityUpdate = queryTempCustUpdate.City;
                resultPostalCode = queryTempCustUpdate.PostalCode;

                //Remove temp customer entry
                var tempCustomer = db.Customers.Where(c => c.CustomerId == "TEST1");
                db.RemoveRange(tempCustomer);
                db.SaveChanges();
            }
            Assert.That($"{resultCityUpdate}, {resultPostalCode}", Is.EqualTo($"New York, BOX101"));


        }

        [Test]
        public void WhenACustomerIsUpdated_SelectedCustomerIsUpdated()
        {
            _customerManager.Delete("TEST1");
            _customerManager.Create("TEST1", "Henry Ford", "Ford CO.", "Detroit");  //create temp entry
            _customerManager.Update("TEST1", "Henry Ford", "Ford CO.", "New York", "BOX101");
            string resultPostalCode = "";
            string resultCityUpdate = "";
            using (var db = new NorthwindContext())
            {
                var queryTempCustUpdate =
                    db.Customers.Where(c => c.CustomerId == "TEST1").FirstOrDefault();
                resultCityUpdate = queryTempCustUpdate.City;
                resultPostalCode = queryTempCustUpdate.PostalCode;
                // remove test entry
                var tempCustomer = db.Customers.Where(c => c.CustomerId == "TEST1");
                db.RemoveRange(tempCustomer);
                db.SaveChanges();

            }
            Assert.That($"{resultCityUpdate}, {resultPostalCode}", Is.EqualTo($"New York, BOX101"));
        }

        [Test]
        public void WhenACustomerIsNotInTheDatabase_Update_ReturnsFalse()
        {
            bool isCustTableUpdated = _customerManager.Update("TEST1", "Henry Ford", "Ford CO."
                , "New York", "BOX101");
            Assert.IsFalse(isCustTableUpdated);
        }

        [Test]
        public void WhenACustomerIsRemoved_TheNumberOfCustomersDecreasesBy1()
        {
            _customerManager.Delete("TEST");
            //Add customer, add temp before count
            _customerManager.Create("TEST", "Temp Entry", "McTemp", "Rivendale");
            int beforeDelCount;
            int countAfterRemovingNewCustomer;
            using (var db = new NorthwindContext())
            {
                //check count

                beforeDelCount =
                    db.Customers.Count();
                
                //Remove temp customer entry and get count
                var tempCustomer = db.Customers.Where(c => c.CustomerId == "TEST");
                db.RemoveRange(tempCustomer);
                db.SaveChanges();
                countAfterRemovingNewCustomer =
                    db.Customers.Count();

            }
            Assert.That(beforeDelCount, Is.EqualTo(countAfterRemovingNewCustomer + 1));

        }

        [Test]
        public void WhenACustomerIsRemoved_TheyAreNoLongerInTheDatabase()
        {
            _customerManager.Delete("TEST");
            //create temp entry
            _customerManager.Create("TEST", "Temp Entry", "McTemp", "Rivendale");
            NorthwindData.Customer isEntryInDatabaseQuery;
            using (var db = new NorthwindContext())
            {
                //Remove temp customer entry
                var tempCustomer = db.Customers.Where(c => c.CustomerId == "TEST");
                db.RemoveRange(tempCustomer);
                db.SaveChanges();

                // query for deleted entry
                isEntryInDatabaseQuery = db.Customers.Where(c => c.CustomerId == "TEST").FirstOrDefault();

            }
            Assert.That(isEntryInDatabaseQuery, Is.Null);


        }

        [TearDown]
        public void TearDown()
        {

            using (var db = new NorthwindContext())
            {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "MANDA"
                select c;

                db.Customers.RemoveRange(selectedCustomers);
                db.SaveChanges();
            }
        }
    }
}