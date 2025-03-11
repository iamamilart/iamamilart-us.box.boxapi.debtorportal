using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using US.BOX.BoxAPI.Data.Models;
using US.BOX.BoxAPI.Data.Utils;
using US.BOX.DataAccess.DBFramework;

namespace US.BOX.BoxAPI.Data.Actions
{
    public class GetObjectionProfileAction(int caseNo) : DBActionBase<List<ObjectionProfile>>
    {
        public override async Task<List<ObjectionProfile>> ExecuteAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            List<ObjectionProfile> objectionProfiles = new List<ObjectionProfile>();
            string storedProcedure = "[dpapi].[GetObjectionProfile]";
            try
            {
                using var command = await CreateCommandAsync(storedProcedure, CommandType.StoredProcedure, connectionString, cancellationToken);
                command.Parameters.Add(DataAcessUtils.CreateParam("@Caseno", DbType.Int32, caseNo));

                using var dataReader = await command.ExecuteReaderAsync(cancellationToken);
                int _profileId = 0;
                int _catogoryId = 0;
                string _categoryName = "";
                ObjectionCategory _objectionCategory = new ObjectionCategory();
                ObjectionReason _categoryReason = new ObjectionReason();

                ObjectionProfile objectionProfile = new ObjectionProfile();
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int profileId = (Int32)GetSafeValue(dataReader,"ProfileId");
                    string profileName = (string)GetSafeValue(dataReader,"ProfileName");
                    int categoryId = (Int32)GetSafeValue(dataReader,"CategoryId");
                    string categoryName = (string)GetSafeValue(dataReader,"Description");
                    int reasonId = (Int32)GetSafeValue(dataReader,"ReasonId");
                    string reason = (string)GetSafeValue(dataReader,"Reason");

                    if (_profileId == profileId)
                    {
                        if (categoryId == _catogoryId)
                        {
                            ObjectionReason categoryReason = new ObjectionReason()
                            {
                                ReasonId = reasonId,
                                Reason = reason
                            };
                            _objectionCategory.ReasonList.Add(categoryReason);
                        }
                        else
                        {
                            ObjectionCategory objectionCategory = new ObjectionCategory();
                            _catogoryId = categoryId;
                            _categoryName = categoryName;

                            ObjectionReason categoryReason = new ObjectionReason()
                            {
                                ReasonId = reasonId,
                                Reason = reason
                            };

                            objectionCategory.CategoryId = categoryId;
                            objectionCategory.Category = categoryName;
                            objectionCategory.ReasonList.Add(categoryReason);
                            _objectionCategory = objectionCategory;
                            objectionProfile.ObjectionCategoryList.Add(objectionCategory);
                        }

                    }
                    else
                    {
                        _profileId = profileId;
                        objectionProfile.ProfileId = profileId;
                        objectionProfile.ProfileName = profileName;

                        ObjectionCategory objectionCategory = new ObjectionCategory();

                        if (categoryId == _catogoryId)
                        {
                            ObjectionReason categoryReason = new ObjectionReason()
                            {
                                ReasonId = reasonId,
                                Reason = reason
                            };
                            objectionCategory.ReasonList.Add(categoryReason);
                        }
                        else
                        {
                            _catogoryId = categoryId;
                            _categoryName = categoryName;

                            ObjectionReason categoryReason = new ObjectionReason()
                            {
                                ReasonId = reasonId,
                                Reason = reason
                            };

                            objectionCategory.CategoryId = categoryId;
                            objectionCategory.Category = categoryName;
                            objectionCategory.ReasonList.Add(categoryReason);
                            _objectionCategory = objectionCategory;
                        }
                        objectionProfile.ObjectionCategoryList.Add(objectionCategory);
                    }
                }
                objectionProfiles.Add(objectionProfile);


            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure {storedProcedure} for caseNumber {caseNo}", ex);
            }
            return objectionProfiles;
        }
    }
}
