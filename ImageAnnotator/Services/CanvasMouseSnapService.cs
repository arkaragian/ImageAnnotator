using ImageAnnotator.Model;
using libGeometry;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ImageAnnotator.Services;

/// <summary>
/// A service that calculates if a mouse event should be snapped somewhere.
/// </summary>
public class CanvasMouseSnapService {

    public List<Point> PointCluster { get; set; }

    public List<IAnnotation> Annotations { get; set; } = [];

    public CanvasMouseSnapService() {
        PointCluster = [];
    }

    public bool TrySnap(MathPoint currentMousePoint, double tolerance) {
        foreach (IAnnotation an in Annotations) {
            List<MathPoint> pts = an.SnapPoints;

            foreach (MathPoint p in pts) {
                double dx = p[0] - currentMousePoint[0];
                double dy = p[1] - currentMousePoint[1];
                double dist = Math.Sqrt((dx * dx) + (dy * dy));

                if (dist < tolerance) {
                    Console.WriteLine($"Snap to: '{an.Name}' Dist:{dist:F3} Tol:{tolerance:F3} MPos: ({currentMousePoint[0]:F3},{currentMousePoint[1]:F3}) APos ({p[0]:F3},{p[1]:F3})");
                    return true;
                }
            }
        }

        return false;

        //foreach (Point p in _index.QueryNeighborhood(mouseCanvasPoint, snapToleranceCanvas)) {
        //    double dx = p.X - mouseCanvasPoint.X;
        //    double dy = p.Y - mouseCanvasPoint.Y;
        //    double d2 = dx * dx + dy * dy;
        //    if (d2 <= bestD2) {
        //        bestD2 = d2;
        //        best = p;
        //        found = true;
        //    }
        //}

        //snapped = best;
        //return found;
    }
}