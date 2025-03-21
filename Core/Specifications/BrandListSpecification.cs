using Core.Entities;
using SkiShop.Core.Specifications;

namespace Core.Specifications;

public class BrandListSpecification : BaseSpecifications<Product, string>
{
    public BrandListSpecification()
    {
        AddSelect(p => p.Brand);
        ApplyDistinct();
    }

}