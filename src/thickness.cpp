/*
 * thickness.cpp: Thickness parsing
 *
 * Contact:
 *   Moonlight List (moonlight-list@lists.ximian.com)
 *
 * Copyright 2007 Novell, Inc. (http://www.novell.com)
 *
 * See the LICENSE file included with the distribution for details.
 * 
 */

#include <config.h>
#include <string.h>
#include <stdlib.h>
#include <math.h>

#include "utils.h"
#include "thickness.h"

bool
Thickness::FromStr (const char *str, Thickness *thickness)
{
	GArray *values = double_garray_from_str (str, 4);
	bool rv = true;

	switch (values->len) {
	case 1:
		*thickness = Thickness (g_array_index (values, double, 0));
		break;
	case 2:
		*thickness = Thickness (g_array_index (values, double, 0),
					g_array_index (values, double, 1));
		break;
	case 3:
		g_warning ("Thickness specified with 3 values, '%s'.", str);
		rv = false;
		break;
	case 4:
		*thickness = Thickness (g_array_index (values, double, 0),
					g_array_index (values, double, 1),
					g_array_index (values, double, 2),
					g_array_index (values, double, 3));
		break;
	}

	if (values)
		g_array_free (values, TRUE);
	return rv;
}
