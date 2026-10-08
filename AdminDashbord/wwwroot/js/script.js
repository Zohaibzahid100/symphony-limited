'use strict';

/* ============================================================
   SYMPHONY LIMITED — ADMIN PANEL
   Global Script (static helpers only)
   ============================================================ */

document.addEventListener('DOMContentLoaded', function () {

  /* ---- Topbar shadow on scroll ---- */
  var topbar = document.querySelector('.dash-topbar');
  if (topbar) {
    window.addEventListener('scroll', function () {
      if (window.scrollY > 8) {
        topbar.style.boxShadow = '0 4px 20px rgba(0,0,0,0.35)';
      } else {
        topbar.style.boxShadow = 'none';
      }
    }, { passive: true });
  }

  /* ---- Table search / filter (client-side row hide, no data injection) ---- */
  document.querySelectorAll('[data-table-search]').forEach(function (input) {
    var targetId = input.getAttribute('data-table-search');
    var table = document.getElementById(targetId);
    if (!table) return;
    var rows = table.querySelectorAll('tbody tr');

    input.addEventListener('keyup', function () {
      var term = input.value.trim().toLowerCase();
      rows.forEach(function (row) {
        var text = row.textContent.toLowerCase();
        row.style.display = text.indexOf(term) !== -1 ? '' : 'none';
      });
    });
  });

});
