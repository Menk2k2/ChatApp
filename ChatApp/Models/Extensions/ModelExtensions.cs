﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ChatApp.Models;
using System.Text.RegularExpressions;

namespace ChatApp.Models.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Chuyển đổi ApplicationUser sang AccountViewModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static AccountViewModel ToAccountVM(this ApplicationUser user)
        {
            return new AccountViewModel()
            {
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FullName = user.FullName
            };
        }

        /// <summary>
        /// Render nút phân quyền
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <param name="userroles"></param>
        /// <returns></returns>
        //public static string RenderButtonSchoolRole(this SchoolUser user, List<Role> roles, string userroles)
        //{
        //    GaiAEntities db = new GaiAEntities();
        //    var s = new StringBuilder();
        //    foreach (var role in roles)
        //    {
        //        if (db.SchoolUserRoles.Any(x => x.SchoolUserID == user.ID && x.RoleID == role.ID))
        //        {
        //            s.AppendFormat("<a class='btn btn-xs btn-danger' title='Xóa quyền {0}' href='#' onclick='return delRole(\"{1}\",\"{0}\",\"{2}\")'><i class='fa fa-times'></i> {2}</a> &nbsp; ", role.ID, user.ID, role.Name);
        //        }
        //        else
        //        {
        //            s.AppendFormat("<a class='btn btn-xs btn-info' title='Thêm quyền {0}' href='#' onclick='return addRole(\"{1}\",\"{0}\",\"{2}\")'><i class='fa fa-plus'></i> {2}</a> &nbsp;", role.ID, user.ID, role.Name);
        //        }
        //    }

        //    return s.ToString();
        //}

        /// <summary>
        /// Render nút phân quyền
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <param name="userroles"></param>
        /// <returns></returns>
        public static string RenderButtonRole(this ApplicationUser user, List<IdentityRole> roles, ICollection<IdentityUserRole> userroles)
        {
            var s = new StringBuilder();
            foreach (var role in roles)
            {
                if (userroles.Any(r => r.RoleId.Equals(role.Id)))
                {
                    s.AppendFormat("<a class='btn btn-xs btn-danger' title='Xóa quyền {0}' href='#' onclick='return delRole(\"{1}\",\"{0}\")'><i class='fa fa-times'></i> {0}</a> &nbsp; ", role.Name, user.Id);
                }
                else
                {
                    s.AppendFormat("<a class='btn btn-xs btn-info' title='Thêm quyền {0}' href='#' onclick='return addRole(\"{1}\",\"{0}\")'><i class='fa fa-plus'></i> {0}</a> &nbsp;", role.Name, user.Id);
                }
            }

            return s.ToString();
        }
    }
}