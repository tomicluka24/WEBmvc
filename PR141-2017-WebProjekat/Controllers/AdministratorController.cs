﻿using PR141_2017_WebProjekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PR141_2017_WebProjekat.Controllers
{
    public class AdministratorController : Controller
    {
        // GET: Administrator
        public ActionResult Index(Korisnik k)
        {
            return View(k);
        }
    }
}