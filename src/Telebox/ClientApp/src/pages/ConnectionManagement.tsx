/* eslint-disable no-restricted-globals */
import { FC, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "react-query";
import { useNavigate, useParams } from "react-router-dom";
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CircularProgress,
  Container,
  Drawer,
  FormControl,
  Grid,
  IconButton,
  List,
  ListItem,
  TextField,
  Typography,
} from "@mui/material";
import StarBorderIcon from "@mui/icons-material/StarBorder";
import CloseIcon from "@mui/icons-material/Close";
import moment from "moment";
import MainLayout from "layouts/MainLayout";
import ConnectionService from "services/ConnectionService";
import ConnectionXStreamService from "services/ConnectionXStreamService";
import RecordingService from "services/RecordingService";
import { IChannel, ICategory } from "models/XStream";
import {
  IScheduleRecordingRequest,
  epgListingToScheduleRecordingRequest,
} from "models/Recording";

const queryStaleTime = 5 * 60 * 1000;

const ConnectionManagement: FC = () => {
  const { connectionId } = useParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { data: recordings } = useQuery("getAllRecordings", () =>
    recordingService.getAll("0")
  );

  const connectionService = new ConnectionService();
  const connectionXStreamService = new ConnectionXStreamService();
  const recordingService = new RecordingService();

  const [categorySearchText, setCategorySearchText] = useState("");
  const [channelSearchText, setChannelSearchText] = useState("");
  const [selectedCategory, setSelectedCategory] = useState<ICategory | null>(
    null
  );
  const [selectedChannel, setSelectedChannel] = useState<IChannel | null>(null);

  const { data: connection, isLoading: isConnectionLoading } = useQuery(
    `getConnection-${connectionId}`,
    async () => await connectionService.getById(connectionId!),
    {
      enabled: connectionId !== undefined,
      refetchOnWindowFocus: false,
    }
  );

  const { data: categories, isLoading: isCategoriesLoading } = useQuery(
    `getCategories-Connection-${connectionId}`,
    async () => await connectionXStreamService.getCategories(connectionId!),
    {
      enabled: connectionId !== undefined,
      staleTime: queryStaleTime,
      refetchOnWindowFocus: false,
    }
  );

  const { data: channels, isLoading: isChannelsLoading } = useQuery(
    `getChannels-Category-${selectedCategory?.id}-Connection-${connectionId}`,
    async () =>
      await connectionXStreamService.getChannels(
        connectionId!,
        selectedCategory!.id
      ),
    {
      enabled: connectionId !== undefined && selectedCategory !== null,
      staleTime: queryStaleTime,
      refetchOnWindowFocus: false,
    }
  );

  const { data: epg, isLoading: isEpgLoading } = useQuery(
    `getChannelEpg-Channel-${selectedChannel?.streamId}-Connection-${connectionId}`,
    async () =>
      await connectionXStreamService.getChannelEpg(
        connectionId!,
        selectedCategory!.id,
        selectedChannel!.streamId
      ),
    {
      enabled:
        connectionId !== undefined &&
        selectedCategory !== null &&
        selectedChannel !== null,
      staleTime: queryStaleTime,
      refetchOnWindowFocus: false,
    }
  );

  const { mutate: scheduleRecording } = useMutation(
    (recording: IScheduleRecordingRequest) =>
      recordingService.schedule(connectionId!, recording),
    {
      onSuccess: () => {
        queryClient.invalidateQueries("getAllRecordings");
      },
    }
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

  return (
    <MainLayout>
      {isConnectionLoading && <CircularProgress />}
      {connection && (
        <>
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <Typography
              sx={{ mb: 2, flexGrow: 1 }}
              variant="h5"
              fontWeight="bold"
            >
              Connection - {connection.name} ({connection.serverUrl})
            </Typography>
            <IconButton onClick={() => navigate("/")}>
              <CloseIcon />
            </IconButton>
          </Box>

          <Grid container spacing={2}>
            <Grid item md={4}>
              <Card>
                <CardContent>
                  <Typography sx={{ mb: 1 }} variant="h6" fontWeight="bold">
                    Categories
                  </Typography>
                  {isCategoriesLoading && <CircularProgress />}
                  {categories && categories.length === 0 && (
                    <Typography>No categories found.</Typography>
                  )}
                  {categories && categories.length > 0 && (
                    <>
                      <FormControl fullWidth>
                        <TextField
                          placeholder="Filter categories"
                          onChange={(e) =>
                            setCategorySearchText(e.target.value.toLowerCase())
                          }
                        />
                      </FormControl>
                      <List>
                        {categories
                          .filter(
                            (x) =>
                              x.name.toLowerCase().indexOf(categorySearchText) >
                              -1
                          )
                          .map((c) => (
                            <ListItem
                              sx={{ cursor: "pointer" }}
                              onClick={() => setSelectedCategory(c)}
                              key={c.id}
                              secondaryAction={
                                <IconButton edge="end" aria-label="favorite">
                                  <StarBorderIcon />
                                </IconButton>
                              }
                            >
                              {c.id === selectedCategory?.id ? (
                                <b>{c.name}</b>
                              ) : (
                                c.name
                              )}
                            </ListItem>
                          ))}
                      </List>
                    </>
                  )}
                </CardContent>
              </Card>
            </Grid>

            <Grid item md={8}>
              <Card>
                <CardContent>
                  <Typography sx={{ mb: 1 }} variant="h6" fontWeight="bold">
                    Channels{" "}
                    {selectedCategory && <>({selectedCategory.name})</>}
                  </Typography>
                  {isChannelsLoading && <CircularProgress />}
                  {channels && channels.length === 0 && (
                    <Typography>No channels found.</Typography>
                  )}
                  {channels && channels.length > 0 && (
                    <>
                      <FormControl fullWidth>
                        <TextField
                          placeholder="Filter channels"
                          onChange={(e) =>
                            setChannelSearchText(e.target.value.toLowerCase())
                          }
                        />
                      </FormControl>
                      <List>
                        {channels
                          .filter(
                            (x) =>
                              x.name.toLowerCase().indexOf(channelSearchText) >
                              -1
                          )
                          .map((c) => (
                            <ListItem
                              key={c.id}
                              sx={{ cursor: "pointer" }}
                              onClick={() => setSelectedChannel(c)}
                              secondaryAction={
                                <IconButton edge="end" aria-label="favorite">
                                  <StarBorderIcon />
                                </IconButton>
                              }
                            >
                              {c.name}
                            </ListItem>
                          ))}
                      </List>
                    </>
                  )}
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          <Drawer
            anchor="bottom"
            open={selectedChannel !== null}
            disableEscapeKeyDown
          >
            {selectedChannel && (
              <Container maxWidth="lg" sx={{ p: 2, overflow: "hidden" }}>
                <Box sx={{ display: "flex", alignItems: "center" }}>
                  <Typography
                    variant="h6"
                    fontWeight="bold"
                    sx={{ flexGrow: 1 }}
                  >
                    {selectedChannel.name}
                  </Typography>
                  <IconButton onClick={() => setSelectedChannel(null)}>
                    <CloseIcon />
                  </IconButton>
                </Box>
                <Box
                  sx={{
                    maxHeight: "450px",
                    overflowY: "scroll",
                    scrollbarWidth: 0,
                    p: 2,
                  }}
                >
                  {isEpgLoading && <CircularProgress />}
                  {epg && epg.length === 0 && (
                    <Typography>No EPG available.</Typography>
                  )}
                  {epg && epg.length > 0 && (
                    <>
                      {epg.map((e) => (
                        <Card sx={{ mb: 2 }}>
                          <CardContent>
                            <Typography fontWeight={"bold"}>
                              {e.title}
                            </Typography>
                            <Typography fontStyle="italic" color="GrayText">
                              {moment(e.start).format("HH:mm")} -{" "}
                              {moment(e.end).format("HH:mm")}
                            </Typography>
                            <Typography>{e.description}</Typography>
                          </CardContent>
                          <CardActions>
                            {recordings?.find(
                              (x) =>
                                x.epgId.toString() === e.id && x.epgId !== 0
                            ) !== undefined ? (
                              <Button
                                onClick={() => {
                                  const recording = recordings.find(
                                    (x) =>
                                      x.epgId.toString() === e.id &&
                                      x.epgId !== 0
                                  )!;
                                  if (
                                    confirm(
                                      "Are you sure you want to cancel this recording?"
                                    )
                                  ) {
                                    removeRecording({
                                      connectionId:
                                        recording.connectionId.toString(),
                                      recordingId: recording.id,
                                    });
                                  }
                                }}
                              >
                                Cancel Recording
                              </Button>
                            ) : (
                              <Button
                                onClick={() => {
                                  scheduleRecording(
                                    epgListingToScheduleRecordingRequest(
                                      selectedChannel,
                                      e
                                    )
                                  );
                                }}
                              >
                                Record
                              </Button>
                            )}
                          </CardActions>
                        </Card>
                      ))}
                    </>
                  )}
                </Box>
              </Container>
            )}
          </Drawer>
        </>
      )}
    </MainLayout>
  );
};

export default ConnectionManagement;
