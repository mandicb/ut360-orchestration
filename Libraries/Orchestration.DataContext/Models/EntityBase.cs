using System.ComponentModel.DataAnnotations;

namespace Orchestration.DataContext.Models;

public abstract class EntityBase
{
    #region Constructors

    protected EntityBase() { }

    protected EntityBase(int id)
    {
        Id = id;
    }

    #endregion Constructors
    
    #region Properties

    
    [Key]
    public int Id { get; set; }

    #endregion
}