namespace OpenCvRuntimeEditor.Core
{
    public interface IRuntimeDataContainer
    {
        T GetData<T>();
        void SetData(object data, bool recalculate = false);
    }
}
