/* eslint-disable no-restricted-globals */
import { FC, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { useNavigate } from "react-router-dom";
import {
  Button,
  Card,
  CardActions,
  CardContent,
  CircularProgress,
  Grid,
  IconButton,
  List,
  ListItem,
  ListItemText,
  Typography,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import ExitToAppIcon from "@mui/icons-material/ExitToApp";
import AccessAlarmIcon from "@mui/icons-material/AccessAlarm";
import FiberManualRecordIcon from "@mui/icons-material/FiberManualRecord";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import MainLayout from "layouts/MainLayout";
import CreateConnectionDialog from "components/CreateConnectionDialog";
import ConnectionService from "services/ConnectionService";
import RecordingService from "services/RecordingService";
import { RecordingStatus } from "models/Recording";
import moment from "moment";

const Dashboard: FC = () => {
  const connectionService = new ConnectionService();
  const recordingService = new RecordingService();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [createConnectionDialogOpen, setCreateConnectionDialogOpen] =
    useState(false);

  const { data: connections, isLoading: isConnectionsLoading } = useQuery(
    "getAllConnections",
    connectionService.getAll
  );

  const { data: recordings, isLoading: isRecordingsLoading } = useQuery(
    "getAllRecordings",
    () => recordingService.getAll("0")
  );

  const { mutate: removeRecording } = useMutation(
    (d: { connectionId: string; recordingId: number }) =>
      recordingService.remove(d.connectionId, d.recordingId),
    {
      onSuccess: () => {
        queryClient.invalidateQueries("getAllRecordings");
      },
    }
  );

  const onCreateConnectionDialogClose = () => {
    setCreateConnectionDialogOpen(false);
  };

  return (
    <MainLayout>
      <Grid container spacing={2}>
        <Grid item md={6}>
          <Card sx={{ minWidth: 275 }}>
            <CardContent>
              <Typography fontWeight="bold" variant="h5" gutterBottom>
                Connections
              </Typography>
              {isConnectionsLoading && <CircularProgress />}
              {connections && connections.length > 0 && (
                <List>
                  {connections.map((c) => (
                    <ListItem
                      secondaryAction={
                        <>
                          <IconButton
                            onClick={() => {
                              navigate(`/connections/${c.id}`);
                            }}
                            edge="end"
                            aria-label="goto"
                            sx={{ mr: 0 }}
                          >
                            <ExitToAppIcon />
                          </IconButton>
                          <IconButton edge="end" aria-label="delete">
                            <DeleteIcon />
                          </IconButton>
                        </>
                      }
                    >
                      <ListItemText primary={c.name} />
                    </ListItem>
                  ))}
                </List>
              )}
              {connections && connections.length === 0 && (
                <Typography>You have no connections.</Typography>
              )}
            </CardContent>
            <CardActions>
              <Button onClick={() => setCreateConnectionDialogOpen(true)}>
                Add Connection
              </Button>
            </CardActions>
          </Card>
        </Grid>

        <Grid item md={6}>
          <Card sx={{ minWidth: 275 }}>
            <CardContent>
              <Typography fontWeight="bold" variant="h5" gutterBottom>
                Recordings
              </Typography>
              {isRecordingsLoading && <CircularProgress />}
              {recordings && recordings.length > 0 && (
                <List>
                  {recordings.map((c) => (
                    <ListItem
                      secondaryAction={
                        <>
                          <IconButton
                            edge="end"
                            aria-label="status"
                            sx={{ mr: 0 }}
                          >
                            {c.status === RecordingStatus.Scheduled && (
                              <AccessAlarmIcon />
                            )}
                            {c.status === RecordingStatus.Recording && (
                              <FiberManualRecordIcon color="error" />
                            )}
                            {c.status === RecordingStatus.Recorded && (
                              <CheckCircleOutlineIcon />
                            )}
                            {c.status === RecordingStatus.Errored && (
                              <ErrorOutlineIcon />
                            )}
                          </IconButton>
                          <IconButton
                            edge="end"
                            aria-label="delete"
                            onClick={() => {
                              if (
                                confirm(
                                  "Are you sure you want to delete this recording?"
                                )
                              ) {
                                removeRecording({
                                  connectionId: c.connectionId.toString(),
                                  recordingId: c.id,
                                });
                              }
                            }}
                          >
                            <DeleteIcon />
                          </IconButton>
                        </>
                      }
                    >
                      <ListItemText
                        primary={c.name}
                        secondary={`${moment(c.startTime).format(
                          "DD/MM/yyyy"
                        )} ${moment(c.startTime).format("HH:mm")} - ${moment(
                          c.endTime
                        ).format("HH:mm")}`}
                      />
                    </ListItem>
                  ))}
                </List>
              )}
              {recordings && recordings.length === 0 && (
                <Typography>You have no recordings.</Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <CreateConnectionDialog
        open={createConnectionDialogOpen}
        onClose={onCreateConnectionDialogClose}
      />
    </MainLayout>
  );
};

export default Dashboard;
