using Core.Entities;
using SkiShop.Core.Specifications;

namespace Core.Specifications;

public class TypeListSpecification : BaseSpecifications<Product, string>
{
    public TypeListSpecification()
    {
        AddSelect(p => p.Type);
        ApplyDistinct();
    }

}