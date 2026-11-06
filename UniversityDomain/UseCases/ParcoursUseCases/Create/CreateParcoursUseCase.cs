using System.Text.RegularExpressions;
using UniversityDomain.DataAdapters;
using UniversityDomain.Entities;
using UniversityDomain.DataAdapters.DataAdaptersFactory;
using UniversityDomain.Entities;
using UniversityDomain.Exceptions.ParcoursExceptions;

namespace UniversityDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory factory)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours{NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pc = await factory.ParcoursRepository().CreateAsync(parcours);
        factory.SaveChangesAsync().Wait();
        return pc;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        
        //nom non null, pas même nom et année, pas de caractère spéciaux
        if (string.IsNullOrWhiteSpace(parcours.NomParcours))
        {
            throw new NotNullNomParcoursException(parcours.NomParcours + " - le nom du parcours ne doit pas être vide.");
        }
        else if (!Regex.IsMatch(parcours.NomParcours.Trim(), @"^[\p{L}\p{N} '\-]+$"))
        {
            throw new InvalidNomParcoursException(parcours.NomParcours + " - le nom du parcours ne doit pas contenir de caractères spéciaux.");
        }
        
        List<Parcours> existant = await factory.ParcoursRepository().FindByConditionAsync(p =>p.NomParcours.Trim().ToLower() == parcours.NomParcours.Trim().ToLower()
            && p.AnneeFormation == parcours.AnneeFormation);
        
        if (existant is {Count:>0}) throw new DuplicateParcoursException(parcours.NomParcours + " - ce parcours existe déjà pour cette année de formation.");
        
    }
}