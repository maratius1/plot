namespace Plot.Sample.Data.Nodes
{
    public class VehicleNode : AssetTypeNode
    {
        public VehicleNode()
        {
            
        }

        public VehicleNode(Vehicle vehicle)
        {
            this["Id"] = vehicle.Id;
            this["TypeName"] = vehicle.TypeName;
        }
        
        public Vehicle AsVehicle()
        {
            return new Vehicle
            {
                Id = this["Id"],
                TypeName = this["TypeName"]
            };
        }
    }
}
