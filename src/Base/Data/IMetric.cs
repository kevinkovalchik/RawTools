namespace Base.Data;

using ThermoFisher.CommonCore.Data.Business;

public abstract class BaseMetric<T>
{
    public abstract string Name { get; }
    public abstract T Value { get; }
    public virtual void Calculate(Scan scan) => throw new NotImplementedException();
    public virtual void Calculate(CentroidStream scan) => throw new NotImplementedException();
    public virtual void Calculate(SegmentedScan scan) => throw new NotImplementedException();
    public virtual void Calculate(Scan scan, double mz) => throw new NotImplementedException();
    public virtual void Calculate(CentroidStream scan, double mz) => throw new NotImplementedException();
    public virtual void Calculate(SegmentedScan scan, double mz) => throw new NotImplementedException();
    public virtual void Calculate(int scanNumber) => throw new NotImplementedException();
    public virtual void Calculate(int scanNumber, double mz) => throw new NotImplementedException();
}