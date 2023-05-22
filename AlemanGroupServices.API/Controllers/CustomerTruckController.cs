using AlemanGroupServices.Core.Models;
using AlemanGroupServices.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AlemanGroupServices.EF;
using static System.Collections.Specialized.BitVector32;

namespace AlemanGroupServices.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTruckController : ControllerBase
    {
        private readonly IStationUnitOfWork
           _stationUnitOfWork;
        public CustomerTruckController(IStationUnitOfWork stationUnitOfWork)
        {
            _stationUnitOfWork = stationUnitOfWork;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                string sql = "select * from tblCustomersTrucks;";
                var trucks = await _stationUnitOfWork.DataAccess.LoadData<TblCustomerTurck, dynamic>(sql, new { });
                return Ok(trucks);
                //return Ok(_stationUnitOfWork.CustomersTrucksRepository.GetAll());
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetStationCustomersTrucks")]
        public async Task<IActionResult> GetStationCustomersTrucks(string stationName)
        {
            try
            {
                string sql = $"CALL GetStationCustomersTrucks('{stationName}');";
                var trucks = await _stationUnitOfWork.DataAccess.LoadData<TblCustomerTurck, dynamic>(sql, stationName);
                return Ok(trucks);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpGet("GetByCustomerID")]
        public async Task<IActionResult> GetByCustomerID(int customerID)
        {
            try
            {
                string sql = $"select * from tblCustomersTrucks where Customer_Id = '{customerID}';";
                var trucks = await _stationUnitOfWork.DataAccess.LoadData<TblCustomerTurck, dynamic>(sql, new { });
                return Ok(trucks);
                //return Ok(_stationUnitOfWork.CustomersTrucksRepository.FindAll(
                //b => b.Customer_Id == customerID));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        //[HttpGet("GetStationTanksOrderedByName")]
        //public IActionResult GetTanksOrderedByName(int customerID)
        //{
        //    try
        //    {
        //        return Ok(_stationUnitOfWork.CustomersTrucksRepository.FindAll(b => b.Customer_Id == customerID
        //        , null, null, b => b.Truck_No));
        //    }
        //    catch (Exception ex) { return BadRequest(ex.ToString()); }
        //}

        [HttpGet("GetCusomerTrucksOrderedByNo")]
        public async Task<IActionResult> getCusomerTrucksOrderedByNo(int customerID)
        {
            try
            {
                string sql = $"select * from tblCustomersTrucks  where Customer_Id = {customerID} ORDER By Truck_No;";
                var trucks = await _stationUnitOfWork.DataAccess.LoadData<TblCustomerTurck, dynamic>(sql, new { });
                return Ok(trucks);
                //return Ok(_stationUnitOfWork.CustomersTrucksRepository.FindAll(b => b.Customer_Id == customerID
                //, null, null, b => b.Truck_No));
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPost("AddcustomerTurck")]
        public async Task<IActionResult> AddcustomerTurck([FromBody] TblCustomerTurck customerTurck)
        {
            try
            {
                string sql =
            "insert into tblCustomersTrucks(Customer_Id, Truck_No) " +
            $"values (@Customer_Id, @Truck_No)";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<TblCustomerTurck>(sql, customerTurck);
                return Ok(customerTurck);
                //var customerTurckTemp = _stationUnitOfWork.CustomersTrucksRepository.Add(customerTurck);
                //_stationUnitOfWork.complete();
                //return Ok(customerTurckTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }

        }

        [HttpPost("AddRangeOfcustomerTurcks")]
        public async Task<IActionResult> AddRangeOfcustomerTurcks([FromBody] List<TblCustomerTurck> customerTurcks)
        {
            try
            {
                string sql =
            "insert into tblCustomersTrucks(Customer_Id, Truck_No) values";
                customerTurcks.ForEach(b => sql += $" ({b.Customer_Id}, '{b.Truck_No}'),");
                sql = sql.Remove(sql.Length - 1, 1);
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<List<TblCustomerTurck>>(sql, customerTurcks);
                return Ok(customerTurcks);
                //List<TblCustomerTurck> fullcustomerTurcks = new List<TblCustomerTurck>();
                //customerTurcks.ForEach(b => fullcustomerTurcks.Add(b));
                //var customerTurcksIEnumable = _stationUnitOfWork.CustomersTrucksRepository.AddRange(customerTurcks);
                //_stationUnitOfWork.complete();
                //return Ok(customerTurcksIEnumable);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }

        [HttpPut("UpdatecustomerTurck")]
        public async Task<IActionResult> UpdatecustomerTurck(int cusotmerID, string truckNo,[FromBody] TblCustomerTurck customerTurck)
        {
            try
            {
                string sql = $"UPDATE tblCustomersTrucks SET  Customer_Id= @Customer_Id, Truck_No = @Truck_No WHERE Customer_Id = {cusotmerID} AND Truck_No = '{truckNo}';";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<TblCustomerTurck>(sql, customerTurck);
                return Ok(customerTurck);
                //var customerTurckTemp = _stationUnitOfWork.CustomersTrucksRepository.Update(customerTurck);
                //_stationUnitOfWork.complete();
                //return Ok(customerTurckTemp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("DeletecustomerTurck")]
        public async Task<IActionResult> DeletecustomerTurck([FromBody] TblCustomerTurck customerTurck)
        {
            try
            {
                string sql = $"DELETE FROM tblCustomersTrucks WHERE Customer_Id = {customerTurck.Customer_Id} And Truck_No = '{customerTurck.Truck_No}'";
                Console.WriteLine(sql);
                await _stationUnitOfWork.DataAccess.SaveData<TblCustomerTurck>(sql, customerTurck);
                return Ok(customerTurck);
                //TblCustomerTurck? customerTurck = _stationUnitOfWork.CustomersTrucksRepository.Find(b => b.Customer_Id == customerTurckNo && b.Truck_No == truckNo);
                //if (customerTurck == null) return Ok(false);
                //var customerTurckTemp = _stationUnitOfWork.CustomersTrucksRepository.Delete(customerTurck);
                //_stationUnitOfWork.complete();
                //return Ok(customerTurckTemp);
            }
            catch (Exception ex) { return BadRequest(ex.ToString()); }
        }
    }
}
